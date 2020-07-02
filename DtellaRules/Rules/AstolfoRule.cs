using ChatBeet;
using DtellaRules.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace DtellaRules.Rules
{
    public class AstolfoRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly TwitterImageService twitterImageService;

        public AstolfoRule(TwitterImageService twitterImageService, IOptions<ChatBeetConfiguration> options)
        {
            this.twitterImageService = twitterImageService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            if (incomingMessage.Content == $"{config.CommandPrefix}astolfo")
            {
                var tweet = await twitterImageService.GetImageTweet("astolfomedia");

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
