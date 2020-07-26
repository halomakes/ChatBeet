using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Rules
{
    public class AstolfoRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly RecentTweetsService twitterImageService;

        public AstolfoRule(RecentTweetsService twitterImageService, IOptions<ChatBeetConfiguration> options)
        {
            this.twitterImageService = twitterImageService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (incomingMessage.Message == $"{config.CommandPrefix}astolfo")
            {
                var tweet = await twitterImageService.GetRecentTweet("astolfomedia");

                var imageUrl = tweet.Entities?.MediaEntities?.FirstOrDefault()?.MediaUrlHttps;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), imageUrl);
                }
            }
        }
    }
}
