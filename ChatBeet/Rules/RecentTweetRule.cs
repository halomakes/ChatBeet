using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using LinqToTwitter;
using System.Threading.Tasks;

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

                yield return await GetResponseAsync(username, incomingMessage.GetResponseTarget());
            }
        }

        private async Task<IClientMessage> GetResponseAsync(string username, string target)
        {
            try
            {
                var tweet = await tweetService.GetRecentTweet(username, false, false);

                return new PrivateMessage(
                    target,
                    tweet != default
                        ? $"{IrcValues.BOLD}{tweet.User?.Name}{IrcValues.RESET} at {tweet.CreatedAt} - {tweet.Text}"
                        : "Sorry, couldn't find anything recent."
                );
            }
            catch (TwitterQueryException)
            {
                return new PrivateMessage(target, "Sorry, couldn't find that account.");
            }
        }
    }
}
