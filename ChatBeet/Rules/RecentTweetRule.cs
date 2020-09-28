using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using LinqToTwitter;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class RecentTweetRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly TwitterService tweetService;
        private readonly Regex rgx;

        public RecentTweetRule(TwitterService tweetService, IOptions<IrcBotConfiguration> options)
        {
            this.tweetService = tweetService;
            config = options.Value;
            rgx = new Regex($"^(?:({Regex.Escape(config.Nick)}, what(?:'|’)?s new from)|({Regex.Escape(config.CommandPrefix)}tweet)) @?([a-zA-Z0-9_]{{1,15}})\\??", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
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

                if (tweet == default)
                {
                    return new PrivateMessage(target, "Sorry, couldn't find anything recent.");
                }
                else
                {
                    return new PrivateMessage(target, tweet.ToIrcMessage());
                }
            }
            catch (TwitterQueryException)
            {
                return new PrivateMessage(target, "Sorry, couldn't find that account.");
            }
        }
    }
}
