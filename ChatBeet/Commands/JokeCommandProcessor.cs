using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class JokeCommandProcessor : CommandProcessor
    {
        private readonly DadJokeService jokeService;

        public JokeCommandProcessor(DadJokeService jokeService)
        {
            this.jokeService = jokeService;
        }

        [Command("joke", Description = "Get a random (bad) joke.")]
        public async Task<IClientMessage> GetJoke()
        {
            var joke = await jokeService.GetDadJokeAsync();

            if (!string.IsNullOrEmpty(joke))
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), joke.Trim());
            else
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), "I'm the joke. 😢");
        }
    }
}
