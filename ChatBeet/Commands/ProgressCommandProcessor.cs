using ChatBeet.Attributes;
using ChatBeet.Configuration;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Humanizer;
using IF.Lastfm.Core.Api.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace ChatBeet.Commands
{
    public class ProgressCommandProcessor : CommandProcessor
    {
        private readonly UserPreferencesService preferences;

        public ProgressCommandProcessor(UserPreferencesService preferences)
        {
            this.preferences = preferences;
        }

        [Command("progress {timeUnit}", Description = "Gets progress over a period of time. Options include year, day, hour, workday, president.")]
        [RateLimit(5, TimeUnit.Minute)]
        public async IAsyncEnumerable<IClientMessage> Respond(string timeUnit)
        {
            if (timeUnit != "workday")
            {
                var bar = GetProgressBar(timeUnit);
                if (!string.IsNullOrEmpty(bar))
                {
                    yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), bar);
                }
            }
            else
            {
                yield return await GetWorkdayProgress();
            }
        }

        private string GetProgressBar(string mode)
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
                    // inauguration is January 20 at noon eastern time every 4 years (year after leap year)
                    var termYears = 4;
                    var startYear = now.Year - (now.Year % termYears) + 1;
                    var easternTimeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
                    var inauguration = new DateTimeOffset(new DateTime(startYear, 1, 20, 12, 0, 0, DateTimeKind.Unspecified), easternTimeZone.BaseUtcOffset);
                    start = (inauguration > now ? inauguration.AddYears(-1 * termYears) : inauguration).DateTime;
                    end = (inauguration > now ? inauguration : inauguration.AddYears(termYears)).DateTime;
                    return Progress.GetBar(now, start, end, $"{IrcValues.BOLD}This presidential term{IrcValues.RESET} is");
                default:
                    BypassRateLimit();
                    return null;
            };
        }

        public async Task<IClientMessage> GetWorkdayProgress()
        {
            var startPref = await preferences.Get(IncomingMessage.From, UserPreference.WorkHoursStart);
            var endPref = await preferences.Get(IncomingMessage.From, UserPreference.WorkHoursEnd);

            if (!IsValidDate(startPref))
            {
                var description = UserPreference.WorkHoursStart.GetAttribute<ParameterAttribute>().DisplayName;
                return new PrivateMessage(IncomingMessage.From, $"No valid value set for preference {IrcValues.ITALIC}{description}{IrcValues.RESET}");
            }
            else if (!IsValidDate(endPref))
            {
                var description = UserPreference.WorkHoursEnd.GetAttribute<ParameterAttribute>().DisplayName;
                return new PrivateMessage(IncomingMessage.From, $"No valid value set for preference {IrcValues.ITALIC}{description}{IrcValues.RESET}");
            }
            else
            {
                var now = DateTime.Now;
                var start = NormalizeTime(DateTime.Parse(startPref), now);
                var end = NormalizeTime(DateTime.Parse(endPref), now);

                if (end < start)
                {
                    // handle overnight shifts
                    if (start < now)
                    {
                        end = end.AddDays(1);
                    }
                    else
                    {
                        start = start.AddDays(-1);
                    }
                }

                if (start <= now && end >= now)
                {
                    var bar = Progress.GetBar(now, start, end, $"{IrcValues.BOLD}Your workday{IrcValues.RESET} is");
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), bar);
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"You are outside of working hours.");
                }
            }

            static bool IsValidDate(string val) => !string.IsNullOrEmpty(val) && DateTime.TryParse(val, out var _);

            static DateTime NormalizeTime(DateTime date, DateTime baseline) =>
                new DateTime(baseline.Year, baseline.Month, baseline.Day, date.Hour, date.Minute, date.Second);
        }
    }
}
