using ChatBeet;
using DtellaRules.Utilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class YearProgressRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private static readonly int barLength = 25;

        public YearProgressRule(IOptions<ChatBeetConfiguration> opts)
        {
            config = opts.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}year.?progress", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Content))
            {
                var now = DateTime.Now;
                var start = new DateTime(now.Year, 1, 1, 0, 0, 0);
                var end = start.AddYears(1);

                var ratio = (now - start) / (end - start);
                var segments = Convert.ToInt32(ratio * barLength);
                var percentage = ratio * 100;

                var filled = string.Concat(Enumerable.Range(0, segments).Select(_ => '█'));
                var empty = string.Concat(Enumerable.Range(0, barLength - segments).Select(_ => '░'));
                var percentageDesc = $"{IrcValues.BOLD}{percentage:F}%".Colorize(Convert.ToInt32(percentage));

                var bar = $"{IrcValues.GREEN}{filled}{IrcValues.GREY}{empty}{IrcValues.RESET}  {IrcValues.BOLD}{now.Year}{IrcValues.RESET} is {percentageDesc} complete.";

                yield return new OutboundIrcMessage
                {
                    Content = bar,
                    Target = incomingMessage.Channel
                };
            }
        }
    }
}
