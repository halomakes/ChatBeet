using ChatBeet.Data;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class WhoDefRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly MemoryCellContext ctx;
        private readonly IrcBotConfiguration config;
        private readonly Regex rgx;

        public WhoDefRule(MemoryCellContext ctx, IOptions<IrcBotConfiguration> options)
        {
            config = options.Value;
            this.ctx = ctx;
            rgx = new Regex($"^({Regex.Escape(config.Nick)}, |{Regex.Escape(config.CommandPrefix)})whodef (.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async override IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var key = match.Groups[2].Value.Trim();

                var cell = await ctx.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());

                if (cell != null)
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"{IrcValues.BOLD}{cell.Key}{IrcValues.RESET} was set by {IrcValues.BOLD}{cell.Author}{IrcValues.RESET}"
                    );
                }
                else
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"I don't have anything for {IrcValues.BOLD}{key}{IrcValues.RESET}."
                    );
                }
            }
        }
    }
}