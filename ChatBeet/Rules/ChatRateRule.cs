using ChatBeet.Commands;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class ChatRateRule : CommandAliasRule<SpeedometerCommandProcessor>
    {
        public ChatRateRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex($"^{Regex.Escape(Configuration.Nick)},? how fast (?:are )?we going\\??", RegexOptions.IgnoreCase);
        }

        protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, SpeedometerCommandProcessor commandProcessor)
        {
            yield return commandProcessor.GetMessageRate(default);
        }
    }
}
