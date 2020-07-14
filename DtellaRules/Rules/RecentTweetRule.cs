using ChatBeet;
using DtellaRules.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class RecentTweetRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly RecentTweetsService tweetService;

        public RecentTweetRule(RecentTweetsService tweetService, IOptions<ChatBeetConfiguration> options)
        {
            this.tweetService = tweetService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.BotName}, what(?:'|’)?s new from @?([a-zA-Z0-9_]{{1,15}})\\??", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Content))
            {
                var username = rgx.Replace(incomingMessage.Content, @"$1");

                var tweet = await tweetService.GetRecentTweet(username, false, false);

                yield return new OutboundIrcMessage
                {
                    Content = tweet != null
                        ? $"{tweet.User?.Name} at {tweet.CreatedAt} - {tweet.Text}"
                        : "Sorry, couldn't find anything recent.",
                    OutputType = IrcMessageType.Message,
                    Target = incomingMessage.Channel
                };
            }
        }
    }
}
