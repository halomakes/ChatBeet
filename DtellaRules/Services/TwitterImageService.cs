using LinqToTwitter;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DtellaRules.Services
{
    public class TwitterImageService
    {
        private readonly DtellaRuleConfiguration.TwitterConfiguration twitterConfig;

        public TwitterImageService(IOptions<DtellaRuleConfiguration> twitterOptions)
        {
            twitterConfig = twitterOptions.Value.Twitter;
        }

        /// <summary>
        /// Get a random recent tweet with an image from an account on Twitter
        /// </summary>
        /// <param name="handle">Handle of username to look up</param>
        /// <returns>Recent tweet with an image attached</returns>
        public async Task<Status> GetImageTweet(string handle)
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
                .Where(s => s.Entities.MediaEntities.Any())
                .Take(10)
                .ToListAsync();

            return tweets.OrderBy(_ => random.Next()).FirstOrDefault();
        }
    }
}
