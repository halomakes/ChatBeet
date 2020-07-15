using ChatBeet;
using DtellaRules.Utilities;
using Humanizer;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var rgx = new Regex($"^{config.CommandPrefix}(year|day|hour|minute|month|decade|century|millenium|week).?progress", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var bar = GetProgressBar(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(bar))
                    yield return new OutboundIrcMessage
                    {
                        Content = bar,
                        Target = incomingMessage.Channel
                    };
            }
        }

        private static string GetProgressBar(string mode)
        {
            var now = DateTime.Now;
            DateTime start;
            DateTime end;
            switch (mode.ToLower())
            {
                case "year":
                    start = new DateTime(now.Year, 1, 1, 0, 0, 0);
                    end = start.AddYears(1);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}{now.Year}{IrcValues.RESET} is");
                case "day":
                    start = new DateTime(now.Year, now.Month, now.Day);
                    end = start.AddDays(1);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}Today{IrcValues.RESET} is");
                case "hour":
                    start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                    end = start.AddHours(1);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}This hour{IrcValues.RESET} is");
                case "minute":
                    start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                    end = start.AddMinutes(1);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}This minute{IrcValues.RESET} is");
                case "month":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}{now:MMMM}{IrcValues.RESET} is");
                case "decade":
                    start = new DateTime(now.Year - (now.Year % 10), 1, 1);
                    end = start.AddYears(10);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}The {start.Year}'s{IrcValues.RESET} are");
                case "week":
                    start = now.StartOfWeek(DayOfWeek.Sunday);
                    end = start.AddDays(7);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
                case "century":
                    start = new DateTime(now.Year - (now.Year % 100), 1, 1);
                    end = start.AddYears(100);
                    var century = (now.Year / 100) + 1;
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}The {century.Ordinalize(new CultureInfo("en-US"))} century{IrcValues.RESET} is");
                case "millenium":
                    start = new DateTime(now.Year - (now.Year % 1000), 1, 1);
                    end = start.AddYears(1000);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}This millenium{IrcValues.RESET} is");
                default:
                    return null;
            };
        }

        private static string GetProgressBar(DateTime now, DateTime start, DateTime end, string periodDescription)
        {
            var ratio = (now - start) / (end - start);
            var segments = Convert.ToInt32(ratio * barLength);
            var percentage = ratio * 100;

            var filled = string.Concat(Enumerable.Range(0, segments).Select(_ => '█'));
            var empty = string.Concat(Enumerable.Range(0, barLength - segments).Select(_ => '░'));
            var percentageDesc = $"{IrcValues.BOLD}{percentage:F}%".Colorize(Convert.ToInt32(percentage));

            var bar = $"{IrcValues.GREEN}{filled}{IrcValues.GREY}{empty}{IrcValues.RESET}  {periodDescription} {percentageDesc} complete.";
            return bar;
        }
    }
}
