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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace ChatBeet.Commands
{
    public class ProgressCommandProcessor : CommandProcessor
    {
        private readonly UserPreferencesService preferences;
        private readonly DateTime now;

        public ProgressCommandProcessor(UserPreferencesService preferences)
        {
            this.preferences = preferences;
            now = DateTime.Now;
        }

        [Command("progress {timeUnit}", Description = "Gets progress over a period of time. Options include year, day, hour, workday, president.")]
        public IClientMessage GetGeneralMessage([Required] string timeUnit) => new NoticeMessage(IncomingMessage.From, "Enter a valid time unit.");

        [Command("progress year", Description = "Get progress for the current year.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetYear()
        {
            var start = new DateTime(now.Year, 1, 1, 0, 0, 0);
            return ProgressResult(start, start.AddYears(1), $"{IrcValues.BOLD}{now.Year}{IrcValues.RESET} is");
        }

        [Command("progress day", Description = "Get progress for the current day.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetDay()
        {
            var start = new DateTime(now.Year, now.Month, now.Day);
            return ProgressResult(start, start.AddDays(1), $"{IrcValues.BOLD}Today{IrcValues.RESET} is");
        }

        [Command("progress hour", Description = "Get progress for the current hour.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetHour()
        {
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            return ProgressResult(start, start.AddHours(1), $"{IrcValues.BOLD}This hour{IrcValues.RESET} is");
        }

        [Command("progress minute", Description = "Get progress for the current minute.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetMinute()
        {
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            return ProgressResult(start, start.AddMinutes(1), $"{IrcValues.BOLD}This minute{IrcValues.RESET} is");
        }

        [Command("progress month", Description = "Get progress for the current month.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetMonth()
        {
            var start = new DateTime(now.Year, now.Month, 1);
            return ProgressResult(start, start.AddMonths(1), $"{IrcValues.BOLD}{now:MMMM}{IrcValues.RESET} is");
        }

        [Command("progress decade", Description = "Get progress for the current decade.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetDecade()
        {
            var start = new DateTime(now.Year - (now.Year % 10), 1, 1);
            return ProgressResult(start, start.AddYears(10), $"{IrcValues.BOLD}The {start.Year}s{IrcValues.RESET} are");
        }

        [Command("progress yatoweek", Description = "Get progress for the current week starting on Monday, as the Japanese gods intended.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetOffsetWeek()
        {
            var start = now.StartOfWeek(DayOfWeek.Monday);
            return ProgressResult(start, start.AddDays(7), $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
        }

        [Command("progress week", Description = "Get progress for the current week.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetWeek()
        {
            var start = now.StartOfWeek(DayOfWeek.Sunday);
            return ProgressResult(start, start.AddDays(7), $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
        }

        [Command("progress century", Description = "Get progress for the current century.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetCentury()
        {
            var start = new DateTime(now.Year - (now.Year % 100), 1, 1);
            var century = (now.Year / 100) + 1;
            return ProgressResult(start, start.AddYears(100), $"{IrcValues.BOLD}The {century.Ordinalize(ChatBeetConfiguration.Culture)} century{IrcValues.RESET} is");
        }

        [Command("progress millennium", Description = "Get progress for the current millennium.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetMillennium()
        {
            var start = new DateTime(now.Year - (now.Year % 1000), 1, 1);
            return ProgressResult(start, start.AddYears(1000), $"{IrcValues.BOLD}This millennium{IrcValues.RESET} is");
        }

        [Command("progress second", Description = "Get progress for the current millennium.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetSecond() =>
            new PrivateMessage(IncomingMessage.GetResponseTarget(), Progress.GetBar((double)now.Millisecond / 1000, $"{IrcValues.BOLD}This second{IrcValues.RESET} is"));

        [Command("progress president", Description = "Get progress for the current US presidential term.")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetPresidentialTerm()
        {
            // inauguration is January 20 at noon eastern time every 4 years (year after leap year)
            var termYears = 4;
            var startYear = now.Year - (now.Year % termYears) + 1;
            var easternTimeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
            var inauguration = new DateTimeOffset(new DateTime(startYear, 1, 20, 12, 0, 0, DateTimeKind.Unspecified), easternTimeZone.BaseUtcOffset);
            var start = (inauguration > now ? inauguration.AddYears(-1 * termYears) : inauguration).DateTime;
            var end = (inauguration > now ? inauguration : inauguration.AddYears(termYears)).DateTime;
            return ProgressResult(start, end, $"{IrcValues.BOLD}This presidential term{IrcValues.RESET} is");
        }

        private IClientMessage ProgressResult(DateTime start, DateTime end, string preFormat) =>
            new PrivateMessage(IncomingMessage.GetResponseTarget(), Progress.GetBar(now, start, end, preFormat));

        [Command("progress workday", Description = "Get progress for your current workday.")]
        [RateLimit(5, TimeUnit.Minute)]
        public async Task<IClientMessage> GetWorkday()
        {
            var startPref = await preferences.Get(IncomingMessage.From, UserPreference.WorkHoursStart);
            var endPref = await preferences.Get(IncomingMessage.From, UserPreference.WorkHoursEnd);

            if (!IsValidDate(startPref))
            {
                var description = UserPreference.WorkHoursStart.GetAttribute<ParameterAttribute>().DisplayName;
                return new NoticeMessage(IncomingMessage.From, $"No valid value set for preference {IrcValues.ITALIC}{description}{IrcValues.RESET}");
            }
            else if (!IsValidDate(endPref))
            {
                var description = UserPreference.WorkHoursEnd.GetAttribute<ParameterAttribute>().DisplayName;
                return new NoticeMessage(IncomingMessage.From, $"No valid value set for preference {IrcValues.ITALIC}{description}{IrcValues.RESET}");
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
                new(baseline.Year, baseline.Month, baseline.Day, date.Hour, date.Minute, date.Second);
        }
    }
}
