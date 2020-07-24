using ChatBeet.Utilities;
using LinqToTwitter;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class RecentTweetsService
    {
        private readonly DtellaRuleConfiguration.TwitterConfiguration twitterConfig;
        private readonly IMemoryCache cache;

        public RecentTweetsService(IOptions<DtellaRuleConfiguration> twitterOptions, IMemoryCache cache)
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

                var auth = new ApplicationOnlyAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore()
                    {
                        ConsumerKey = twitterConfig.ConsumerKey,
                        ConsumerSecret = twitterConfig.ConsumerSecret
                    }
                };

                await auth.AuthorizeAsync();
                var twitterContext = new TwitterContext(auth);

                return await twitterContext.Status
                    .Where(s => s.Type == StatusType.User)
                    .Where(s => s.ScreenName == handle)
                    .Where(s => s.RetweetedStatus.StatusID == 0)
                    .Where(s => s.InReplyToStatusID == 0)
                    .Take(10)
                    .ToListAsync();
            });

            var filtered = tweets.Where(s => !mediaOnly || s.Entities.MediaEntities.Any());

            return randomize
                ? filtered.PickRandom()
                : filtered.FirstOrDefault();
        }
    }
}
