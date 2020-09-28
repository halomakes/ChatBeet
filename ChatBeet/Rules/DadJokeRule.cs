using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class DadJokeRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly DadJokeService jokeService;
        private readonly IrcBotConfiguration config;
        private readonly Regex filter;

        public DadJokeRule(DadJokeService jokeService, IOptions<IrcBotConfiguration> options)
        {
            this.jokeService = jokeService;
            config = options.Value;
            filter = new Regex($"^({Regex.Escape(config.Nick)},? ?tell.*joke)|({Regex.Escape(config.CommandPrefix)}(dad )?joke)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => filter.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                var joke = await jokeService.GetDadJokeAsync();

                if (!string.IsNullOrEmpty(joke))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), joke.Trim());
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"I'm the joke. 😢");
                }
            }
        }
    }
}
