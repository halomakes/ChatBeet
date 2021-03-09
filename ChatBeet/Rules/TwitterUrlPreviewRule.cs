using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class TwitterUrlPreviewRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly TwitterService tweetService;
        private readonly Regex rgx;

        public TwitterUrlPreviewRule(TwitterService tweetService)
        {
            this.tweetService = tweetService;
            rgx = new Regex(@"^(?!.*<.*>.*$).*twitter\.com\/.*\/status(?:es)?\/(\d+)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var postId = match.Groups[1].Value;
                if (ulong.TryParse(postId, out var id))
                {
                    var msg = await GetResponseAsync(id, incomingMessage.GetResponseTarget());
                    if (msg != default)
                    {
                        yield return msg;
                    }
                }
            }
        }

        private async Task<IClientMessage> GetResponseAsync(ulong id, string target)
        {
            try
            {
                var tweet = await tweetService.GetTweet(id);

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
                return default;
            }
        }
    }
}
