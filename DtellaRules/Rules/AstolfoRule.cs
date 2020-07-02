using ChatBeet;
using LinqToTwitter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DtellaRules.Rules
{
    public class AstolfoRule : MessageRuleBase<IrcMessage>
    {
        private readonly DtellaRuleConfiguration.TwitterConfiguration twitterConfig;

        public AstolfoRule(IOptions<DtellaRuleConfiguration> options)
        {
            twitterConfig = options.Value.Twitter;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            if (incomingMessage.Content == "🥕 astolfo")
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
                    .Where(s => s.ScreenName == "astolfomedia")
                    .Where(s => s.Entities.MediaEntities.Any())
                    .Take(5)
                    .ToListAsync();

                var tweet = tweets.OrderBy(_ => random.Next()).FirstOrDefault();

                var imageUrl = tweet.Entities?.MediaEntities?.FirstOrDefault()?.MediaUrlHttps;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = imageUrl,
                        OutputType = IrcMessageType.Message,
                        Target = incomingMessage.Channel
                    };
                }
            }
        }
    }
}
