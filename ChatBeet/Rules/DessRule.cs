using ChatBeet.Commands.Irc;
using ChatBeet.Services;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;

public partial class DessRule : CommandAliasRule<BooruCommandProcessor>
{
    private readonly BooruService _booru;

    [GeneratedRegex(@"^!dess$", RegexOptions.IgnoreCase)]
    private static partial Regex rgx();

    public DessRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider, BooruService booru) : base(options, serviceProvider)
    {
        Pattern = rgx();
        _booru = booru;
    }

    protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, BooruCommandProcessor commandProcessor)
    {
        yield return await commandProcessor.GetRandomSafePost("akatsuki_kirika");
    }
}