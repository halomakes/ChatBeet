using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Rules
{
    public class AstolfoRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly RecentTweetsService twitterImageService;
        private readonly string command;

        public AstolfoRule(RecentTweetsService twitterImageService, IOptions<IrcBotConfiguration> options)
        {
            this.twitterImageService = twitterImageService;
            config = options.Value;
            command = $"{config.CommandPrefix}astolfo";
        }

        public override bool Matches(PrivateMessage incomingMessage) => incomingMessage.Message == command;

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            if (incomingMessage.Message == command)
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
