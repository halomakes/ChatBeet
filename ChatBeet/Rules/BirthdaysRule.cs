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
    public class BirthdaysRule : CommandAliasRule<BirthdayCommandProcessor>
    {
        public BirthdaysRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex($@"^(?:{Regex.Escape(Configuration.Nick)},? (?:when)(?: is|['ʼ]s)? ({RegexUtils.Nick})(?:['ʼ]s?)? birthday\??)", RegexOptions.IgnoreCase);
        }

        protected async override IAsyncEnumerable<IClientMessage> OnMatch(Match match, BirthdayCommandProcessor commandProcessor)
        {
            var naturalGroup = match.Groups[1].Value?.Trim();
            var commandGroup = match.Groups[2].Value?.Trim();
            var nick = string.IsNullOrEmpty(naturalGroup) ? commandGroup : naturalGroup;
            yield return await commandProcessor.LookupBirthday(nick);
        }
    }
}
