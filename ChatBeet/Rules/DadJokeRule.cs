using ChatBeet.Commands.Irc;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class DadJokeRule : CommandAliasRule<JokeCommandProcessor>
    {
        public DadJokeRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex($"^{Regex.Escape(options.Value.Nick)},? ?tell.*joke", RegexOptions.IgnoreCase);
        }

        protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match _, JokeCommandProcessor commandProcessor)
        {
            yield return await commandProcessor.GetJoke();
        }
    }
}
