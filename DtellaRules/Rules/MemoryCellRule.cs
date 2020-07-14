using ChatBeet;
using DtellaRules.Data;
using DtellaRules.Data.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class MemoryCellRule : MessageRuleBase<IrcMessage>
    {
        private readonly DtellaContext ctx;
        private readonly ChatBeetConfiguration config;

        public MemoryCellRule(DtellaContext ctx, IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
            this.ctx = ctx;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var setRgx = new Regex($"^({config.BotName}, |{config.CommandPrefix})remember (.*)=(.*)", RegexOptions.IgnoreCase);
            var setMatch = setRgx.Match(incomingMessage.Content);
            if (setMatch.Success)
            {
                var key = setMatch.Groups[2].Value.Trim().ToLower();
                var value = setMatch.Groups[3].Value.Trim();

                var existingCell = await ctx.MemoryCells.FindAsync(key);
                if (existingCell != null)
                {
                    ctx.MemoryCells.Remove(existingCell);
                    await ctx.SaveChangesAsync();
                }

                ctx.MemoryCells.Add(new MemoryCell
                {
                    Author = incomingMessage.Sender,
                    Key = key,
                    Value = value
                });
                await ctx.SaveChangesAsync();

                yield return new OutboundIrcMessage
                {
                    Content = "Got it! 👍",
                    Target = incomingMessage.Channel
                };

                if (existingCell != null)
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Previous value was {IrcValues.BOLD}{existingCell.Value}{IrcValues.RESET}, set by {existingCell.Author}.",
                        Target = incomingMessage.Channel
                    };
            }

            var getRgx = new Regex($"^({config.BotName}, |{config.CommandPrefix})recall (.*)", RegexOptions.IgnoreCase);
            var getMatch = getRgx.Match(incomingMessage.Content);
            if (getMatch.Success)
            {
                var caseSensitiveKey = getMatch.Groups[2].Value.Trim();

                var cell = await ctx.MemoryCells.FindAsync(caseSensitiveKey.ToLower());

                if (cell != null)
                    yield return new OutboundIrcMessage
                    {
                        Content = $"{IrcValues.BOLD}{caseSensitiveKey}{IrcValues.RESET}: {cell.Value}",
                        Target = incomingMessage.Channel
                    };
                else
                    yield return new OutboundIrcMessage
                    {
                        Content = $"I don't have anything for {IrcValues.BOLD}{caseSensitiveKey}{IrcValues.RESET}.",
                        Target = incomingMessage.Channel
                    };
            }
        }
    };
}