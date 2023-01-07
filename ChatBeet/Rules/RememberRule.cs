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
    public class RememberRule : CommandAliasRule<MemoryCellCommandProcessor>
    {
        public RememberRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex($"^{Regex.Escape(Configuration.Nick)},? remember (.*?)=(.*)", RegexOptions.IgnoreCase);
        }

        protected override IAsyncEnumerable<IClientMessage> OnMatch(Match match, MemoryCellCommandProcessor commandProcessor) =>
            commandProcessor.SetCell(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
    }
}