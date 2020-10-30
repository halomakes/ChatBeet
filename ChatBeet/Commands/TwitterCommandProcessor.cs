using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using LinqToTwitter;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class TwitterCommandProcessor : CommandProcessor
    {
        private readonly TwitterService tweetService;

        public TwitterCommandProcessor(TwitterService tweetService)
        {
            this.tweetService = tweetService;
        }

        [Command("tweet {username}")]
        public async Task<IClientMessage> GetRecentTweet(string username)
        {
            try
            {
                var tweet = await tweetService.GetRecentTweet(username, false, false);

                if (tweet == default)
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find anything recent.");
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), tweet.ToIrcMessage());
                }
            }
            catch (TwitterQueryException)
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find that account.");
            }
        }
    }
}
