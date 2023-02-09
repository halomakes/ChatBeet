using ChatBeet.Commands.Irc;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;

public class SuspectRule : CommandAliasRule<SuspicionCommandProcessor>
{
    private readonly NegativeResponseService _negativeResponseService;
    private readonly SuspicionService _service;

    public SuspectRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider, NegativeResponseService negativeResponseService, SuspicionService service) : base(options, serviceProvider)
    {
        Pattern = new Regex($@"(?:^({RegexUtils.Nick})(?: is)? sus$)");
        _negativeResponseService = negativeResponseService;
        _service = service;
    }

    protected override IAsyncEnumerable<IClientMessage> OnMatch(Match match, SuspicionCommandProcessor commandProcessor) =>
        commandProcessor.IncreaseSuspicion(match.Groups[1].Value);
}