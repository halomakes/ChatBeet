using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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
            rgx = new Regex($"^(?:({Regex.Escape(config.Nick)}, what(?:'|’)?s new from)|({Regex.Escape(config.CommandPrefix)}tweet)) @?([a-zA-Z0-9_]{{1,15}})\\??", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var username = match.Groups[3].Value;

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
