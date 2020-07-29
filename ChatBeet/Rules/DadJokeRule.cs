using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class DadJokeRule : MessageRuleBase<PrivateMessage>
    {
        private readonly DadJokeService jokeService;
        private readonly IrcBotConfiguration config;

        public DadJokeRule(DadJokeService jokeService, IOptions<IrcBotConfiguration> options)
        {
            this.jokeService = jokeService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^({config.Nick},? ?tell.*joke)|({config.CommandPrefix}(dad )?joke)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Message);
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
