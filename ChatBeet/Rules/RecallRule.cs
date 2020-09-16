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
    public class RecallRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly MemoryCellContext ctx;
        private readonly IrcBotConfiguration config;
        private readonly Regex commandingRgx;
        private readonly Regex interrogativeRgx;

        public RecallRule(MemoryCellContext ctx, IOptions<IrcBotConfiguration> options)
        {
            config = options.Value;
            this.ctx = ctx;
            commandingRgx = new Regex($"^(?:{Regex.Escape(config.Nick)},? |{Regex.Escape(config.CommandPrefix)})(?:recall|tell me about|show me) (.+)", RegexOptions.IgnoreCase);
            interrogativeRgx = new Regex($"^(?:{Regex.Escape(config.Nick)},? |{Regex.Escape(config.CommandPrefix)})(?:what['ʼ]?(?:s|re)|(?:what|who) (?:is|are)|what do you know about) (.+)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => commandingRgx.IsMatch(incomingMessage.Message) || interrogativeRgx.IsMatch(incomingMessage.Message);

        public async override IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var key = GetKey(incomingMessage.Message);
            if (!string.IsNullOrEmpty(key))
            {
                var cell = await ctx.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());

                if (cell != null)
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"{IrcValues.BOLD}{cell.Key}{IrcValues.RESET}: {cell.Value}"
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

        private string GetKey(string m)
        {
            var commandMatch = commandingRgx.Match(m);
            if (commandMatch.Success)
                return commandMatch.Groups[1].Value.Trim().RemoveLastCharacter('.');

            var questionMatch = interrogativeRgx.Match(m);
            if (questionMatch.Success)
                return questionMatch.Groups[1].Value.Trim().RemoveLastCharacter('?');

            return default;
        }
    }
}