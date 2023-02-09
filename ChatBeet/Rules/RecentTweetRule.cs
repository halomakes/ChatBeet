using ChatBeet.Commands.Irc;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;

public class RecentTweetRule : CommandAliasRule<TwitterCommandProcessor>
{
    public RecentTweetRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
        Pattern = new Regex($"^(?:({Regex.Escape(Configuration.Nick)}, what(?:'|’)?s new from)) @?([a-zA-Z0-9_]{{1,15}})\\??", RegexOptions.IgnoreCase);
    }

    protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, TwitterCommandProcessor commandProcessor)
    {
        var username = match.Groups[2].Value;
        yield return await commandProcessor.GetRecentTweet(username);
    }
}