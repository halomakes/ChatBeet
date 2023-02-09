using ChatBeet.Commands.Irc;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;

public class CommandingRecallRule : CommandAliasRule<MemoryCellCommandProcessor>
{
    public CommandingRecallRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
        Pattern = new Regex($"^(?:{Regex.Escape(Configuration.Nick)},? )(?:recall|tell me about|show me) (.+)", RegexOptions.IgnoreCase);
    }

    protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, MemoryCellCommandProcessor commandProcessor)
    {
        var key = match.Groups[1].Value.Trim().RemoveLastCharacter('.');
        yield return await commandProcessor.GetCell(key);
    }
}