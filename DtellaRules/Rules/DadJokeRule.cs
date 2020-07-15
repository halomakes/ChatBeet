using ChatBeet;
using DtellaRules.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class DadJokeRule : MessageRuleBase<IrcMessage>
    {
        private readonly DadJokeService jokeService;
        private readonly ChatBeetConfiguration config;

        public DadJokeRule(DadJokeService jokeService, IOptions<ChatBeetConfiguration> options)
        {
            this.jokeService = jokeService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^({config.BotName},? ?tell.*joke)|({config.CommandPrefix}(dad )?joke)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var joke = await jokeService.GetDadJokeAsync();

                if (!string.IsNullOrEmpty(joke))
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = joke.Trim(),
                        Target = incomingMessage.Channel
                    };
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"I'm the joke. 😢",
                        Target = incomingMessage.Channel
                    };
                }
            }
        }
    }
}
