using ChatBeet.Configuration;
using ChatBeet.Utilities;
using LinqToTwitter;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class TwitterService
    {
        private readonly ChatBeetConfiguration.TwitterConfiguration twitterConfig;
        private readonly IMemoryCache cache;

        public TwitterService(IOptions<ChatBeetConfiguration> twitterOptions, IMemoryCache cache)
        {
            twitterConfig = twitterOptions.Value.Twitter;
            this.cache = cache;
        }

        /// <summary>
        /// Get a random recent tweet with an image from an account on Twitter
        /// </summary>
        /// <param name="handle">Handle of username to look up</param>
        /// <param name="count">Number of results to fetch</param>
        /// <param name="randomize">Whether or not to pick a random result from the set</param>
        /// <returns>Recent tweet with an image attached</returns>
        public async Task<Status> GetRecentTweet(string handle, bool mediaOnly = true, bool randomize = true)
        {
            var tweets = await cache.GetOrCreateAsync($"twitter:{handle}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                var twitterContext = await GetContext();

                return await twitterContext.Status
                    .Where(s => s.Type == StatusType.User)
                    .Where(s => s.ScreenName == handle)
                    .Where(s => s.RetweetedStatus.StatusID == 0)
                    .Where(s => s.InReplyToStatusID == 0)
                    .Where(s => s.TweetMode == TweetMode.Extended)
                    .Take(10)
                    .ToListAsync();
            });

            var filtered = tweets.Where(s => !mediaOnly || s.Entities.MediaEntities.Any());

            return randomize
                ? filtered.PickRandom()
                : filtered.FirstOrDefault();
        }

        /// <summary>
        /// Get a tweet by its ID
        /// </summary>
        /// <param name="id">ID of the tweet</param>
        public async Task<Status> GetTweet(ulong id) => await cache.GetOrCreateAsync($"twitter:status:{id}", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            var twitterContext = await GetContext();

            return await twitterContext.Status
                .Where(s => s.Type == StatusType.Show)
                .Where(s => s.StatusID == id || s.ID == id)
                .Where(s => s.TweetMode == TweetMode.Extended)
                .FirstOrDefaultAsync();
        });

        private async Task<TwitterContext> GetContext()
        {
            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = twitterConfig.ConsumerKey,
                    ConsumerSecret = twitterConfig.ConsumerSecret
                }
            };

            await auth.AuthorizeAsync();
            return new TwitterContext(auth);
        }
    }
}
