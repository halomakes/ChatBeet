using ChatBeet.Commands;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class SuspectRule : CommandAliasRule<SuspicionCommandProcessor>
    {
        public SuspectRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex($@"(?:^({RegexUtils.Nick})(?: is)? sus$)");
        }

        protected override IAsyncEnumerable<IClientMessage> OnMatch(Match match, SuspicionCommandProcessor commandProcessor) =>
            commandProcessor.IncreaseSuspicion(match.Groups[1].Value);
    }
}
