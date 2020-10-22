using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Humanizer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TimeZoneConverter;

namespace ChatBeet.Rules
{
    public class ProgressRule : IMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;

        public ProgressRule(IOptions<IrcBotConfiguration> opts)
        {
            config = opts.Value;
        }

        public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}progress (year|day|hour|minute|month|decade|century|millennium|week|yatoweek|second|president|presidential term)", RegexOptions.IgnoreCase);
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
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}{now.Year}{IrcValues.RESET} is");
                case "day":
                    start = new DateTime(now.Year, now.Month, now.Day);
                    end = start.AddDays(1);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}Today{IrcValues.RESET} is");
                case "hour":
                    start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                    end = start.AddHours(1);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This hour{IrcValues.RESET} is");
                case "minute":
                    start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
                    end = start.AddMinutes(1);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This minute{IrcValues.RESET} is");
                case "month":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}{now:MMMM}{IrcValues.RESET} is");
                case "decade":
                    start = new DateTime(now.Year - (now.Year % 10), 1, 1);
                    end = start.AddYears(10);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}The {start.Year}s{IrcValues.RESET} are");
                case "yatoweek":
                    start = now.StartOfWeek(DayOfWeek.Monday);
                    end = start.AddDays(7);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
                case "week":
                    start = now.StartOfWeek(DayOfWeek.Sunday);
                    end = start.AddDays(7);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
                case "century":
                    start = new DateTime(now.Year - (now.Year % 100), 1, 1);
                    end = start.AddYears(100);
                    var century = (now.Year / 100) + 1;
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}The {century.Ordinalize(ChatBeetConfiguration.Culture)} century{IrcValues.RESET} is");
                case "millennium":
                    start = new DateTime(now.Year - (now.Year % 1000), 1, 1);
                    end = start.AddYears(1000);
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This millennium{IrcValues.RESET} is");
                case "second":
                    return Progress.GetBar((double)now.Millisecond / 1000, $"{IrcValues.BOLD}This second{IrcValues.RESET} is");
                case "president":
                case "presidential term":
                    // inauguration is July 20 at noon eastern time every 4 years (year after leap year)
                    var termYears = 4;
                    var startYear = now.Year - (now.Year % termYears) + 1;
                    var easternTimeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
                    var inauguration = new DateTimeOffset(new DateTime(startYear, 1, 20, 12, 0, 0, DateTimeKind.Unspecified), easternTimeZone.BaseUtcOffset);
                    start = (inauguration > now ? inauguration.AddYears(-1 * termYears) : inauguration).DateTime;
                    end = (inauguration > now ? inauguration : inauguration.AddYears(termYears)).DateTime;
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This presidential term{IrcValues.RESET} is");
                default:
                    return null;
            };
        }
    }
}
