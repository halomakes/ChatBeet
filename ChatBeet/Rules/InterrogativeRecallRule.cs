using ChatBeet.Commands;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class InterrogativeRecallRule : CommandAliasRule<MemoryCellCommandProcessor>
    {
        public InterrogativeRecallRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex($"^(?:{Regex.Escape(Configuration.Nick)},? )(?:(?!what['ʼ]?s new from)(?:what|who)['ʼ]?(?:s|re)|(?:what|who) (?:is|are)|what do you know about) (.+)", RegexOptions.IgnoreCase);
        }

        protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, MemoryCellCommandProcessor commandProcessor)
        {
            var key = match.Groups[1].Value.Trim().RemoveLastCharacter('?');
            yield return await commandProcessor.GetCell(key);
        }
    }
}