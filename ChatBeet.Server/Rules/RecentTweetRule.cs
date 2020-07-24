using ChatBeet;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class RecentTweetRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly RecentTweetsService tweetService;

        public RecentTweetRule(RecentTweetsService tweetService, IOptions<ChatBeetConfiguration> options)
        {
            this.tweetService = tweetService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.BotName}, what(?:'|’)?s new from @?([a-zA-Z0-9_]{{1,15}})\\??", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Message))
            {
                var username = rgx.Replace(incomingMessage.Message, @"$1");

                var tweet = await tweetService.GetRecentTweet(username, false, false);

                yield return new OutboundIrcMessage
                {
                    Content = tweet != null
                        ? $"{IrcValues.BOLD}{tweet.User?.Name}{IrcValues.RESET} at {tweet.CreatedAt} - {tweet.Text}"
                        : "Sorry, couldn't find anything recent.",
                    OutputType = IrcMessageType.Message,
                    Target = incomingMessage.GetResponseTarget()
                };
            }
        }
    }
}
