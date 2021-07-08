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
    public class DessRule : CommandAliasRule<BooruCommandProcessor>
    {
        public DessRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
        {
            Pattern = new Regex(@"!dess", RegexOptions.IgnoreCase);
        }

        protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match match, BooruCommandProcessor commandProcessor)
        {
            yield return await commandProcessor.GetRandomPost("akatsuki_kirika");
        }
    }
}
