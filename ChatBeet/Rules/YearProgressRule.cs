using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Humanizer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class YearProgressRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private static readonly int barLength = 25;

        public YearProgressRule(IOptions<ChatBeetConfiguration> opts)
        {
            config = opts.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}progress (year|day|hour|minute|month|decade|century|millennium|week|second)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var bar = GetProgressBar(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(bar))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), bar);
                }
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
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}The {century.Ordinalize(DtellaRuleConfiguration.Culture)} century{IrcValues.RESET} is");
                case "millennium":
                    start = new DateTime(now.Year - (now.Year % 1000), 1, 1);
                    end = start.AddYears(1000);
                    return GetProgressBar(now, start, end, $"{IrcValues.BOLD}This millennium{IrcValues.RESET} is");
                case "second":
                    return GetProgressBar((double)now.Millisecond / 1000, $"{IrcValues.BOLD}This second{IrcValues.RESET} is");
                default:
                    return null;
            };
        }

        private static string GetProgressBar(DateTime now, DateTime start, DateTime end, string periodDescription) =>
            GetProgressBar((now - start) / (end - start), periodDescription);

        private static string GetProgressBar(double ratio, string periodDescription)
        {
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
