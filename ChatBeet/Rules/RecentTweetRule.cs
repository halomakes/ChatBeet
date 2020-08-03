using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class RecentTweetRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly RecentTweetsService tweetService;
        private readonly Regex rgx;

        public RecentTweetRule(RecentTweetsService tweetService, IOptions<IrcBotConfiguration> options)
        {
            this.tweetService = tweetService;
            config = options.Value;
            rgx = new Regex($"^{config.Nick}, what(?:'|’)?s new from @?([a-zA-Z0-9_]{{1,15}})\\??", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            if (rgx.IsMatch(incomingMessage.Message))
            {
                var username = rgx.Replace(incomingMessage.Message, @"$1");

                var tweet = await tweetService.GetRecentTweet(username, false, false);

                yield return new PrivateMessage(
                    incomingMessage.GetResponseTarget(),
                    tweet != null
                        ? $"{IrcValues.BOLD}{tweet.User?.Name}{IrcValues.RESET} at {tweet.CreatedAt} - {tweet.Text}"
                        : "Sorry, couldn't find anything recent."
                );
            }
        }
    }
}
