using LinqToTwitter;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DtellaRules.Services
{
    public class RecentTweetsService
    {
        private readonly DtellaRuleConfiguration.TwitterConfiguration twitterConfig;

        public RecentTweetsService(IOptions<DtellaRuleConfiguration> twitterOptions)
        {
            twitterConfig = twitterOptions.Value.Twitter;
        }

        /// <summary>
        /// Get a random recent tweet with an image from an account on Twitter
        /// </summary>
        /// <param name="handle">Handle of username to look up</param>
        /// <param name="count">Number of results to fetch</param>
        /// <param name="randomize">Whether or not to pick a random result from the set</param>
        /// <returns>Recent tweet with an image attached</returns>
        public async Task<Status> GetRecentTweet(string handle, bool mediaOnly = true, int count = 10, bool randomize = true)
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
            var twitterContext = new TwitterContext(auth);
            var random = new Random();

            var tweets = await twitterContext.Status
                .Where(s => s.Type == StatusType.User)
                .Where(s => s.ScreenName == handle)
                .Where(s => s.RetweetedStatus.StatusID == 0)
                .Where(s => s.InReplyToStatusID == 0)
                .Where(s => !mediaOnly || s.Entities.MediaEntities.Any())
                .Take(count)
                .ToListAsync();

            return randomize
                ? tweets.OrderBy(_ => random.Next()).FirstOrDefault()
                : tweets.FirstOrDefault();
        }
    }
}
