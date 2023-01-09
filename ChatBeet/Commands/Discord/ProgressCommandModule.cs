using ChatBeet.Attributes;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Humanizer;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace ChatBeet.Commands.Discord;

[SlashCommandGroup("progress", "Commands related to progress bars")]
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class ProgressCommandModule
{
    private readonly UserPreferencesService preferences;
    private readonly ProgressContext dbContext;
    private readonly DateTime now;

    public ProgressCommandModule(UserPreferencesService preferences, ProgressContext dbContext)
    {
        this.preferences = preferences;
        this.dbContext = dbContext;
        now = DateTime.Now;
    }

    [SlashCommand("custom", "Gets progress over a period of time.")]
    public async Task GetCustomTime([Required] string timeUnit)
    {
        string content = "Enter a valid time unit.";
        var unit = await dbContext.FixedTimeRanges.FirstOrDefaultAsync(r => r.Key.ToLower() == timeUnit.Trim().ToLower());
        if (unit != default)
        {
            var now = DateTime.Now;
            if (now < unit.StartDate)
                content = string.IsNullOrWhiteSpace(unit.BeforeRangeMessage)
                    ? Progress.FormatTemplate(now, unit.StartDate, unit.EndDate, unit.Template)
                    : unit.BeforeRangeMessage;
            else if (now > unit.EndDate)
                content = string.IsNullOrWhiteSpace(unit.AfterRangeMessage)
                    ? Progress.FormatTemplate(now, unit.StartDate, unit.EndDate, unit.Template)
                    : unit.AfterRangeMessage;
            else
                content = Progress.FormatTemplate(now, unit.StartDate, unit.EndDate, unit.Template);
        }
        return new PrivateMessage(IncomingMessage.GetResponseTarget(), content);
    }

    [SlashCommand("year", "Get progress for the current year.")]
    public async Task GetYear()
    {
        var start = new DateTime(now.Year, 1, 1, 0, 0, 0);
        return ProgressResult(start, start.AddYears(1), $"{IrcValues.BOLD}{now.Year}{IrcValues.RESET} is");
    }

    [SlashCommand("day", "Get progress for the current day.")]
    public async Task GetDay()
    {
        var start = new DateTime(now.Year, now.Month, now.Day);
        return ProgressResult(start, start.AddDays(1), $"{IrcValues.BOLD}Today{IrcValues.RESET} is");
    }

    [SlashCommand("yatoday", "Get progress for the current day, but one hour in the past. It's objectively better. ")]
    public async Task GetOffsetDay()
    {
        var nowOffset = now.AddHours(1);
        var start = new DateTime(nowOffset.Year, nowOffset.Month, nowOffset.Day);
        return ProgressResult(start, start.AddDays(1), $"(in the {IrcValues.ITALIC}objectively better{IrcValues.RESET} time zone) {IrcValues.BOLD}Today{IrcValues.RESET} is");
    }

    [SlashCommand("hour", "Get progress for the current hour.")]
    public async Task GetHour()
    {
        var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
        return ProgressResult(start, start.AddHours(1), $"{IrcValues.BOLD}This hour{IrcValues.RESET} is");
    }

    [SlashCommand("minute", "Get progress for the current minute.")]
    public async Task GetMinute()
    {
        var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        return ProgressResult(start, start.AddMinutes(1), $"{IrcValues.BOLD}This minute{IrcValues.RESET} is");
    }

    [SlashCommand("month", "Get progress for the current month.")]
    public async Task GetMonth()
    {
        var start = new DateTime(now.Year, now.Month, 1);
        return ProgressResult(start, start.AddMonths(1), $"{IrcValues.BOLD}{now:MMMM}{IrcValues.RESET} is");
    }

    [SlashCommand("decade", "Get progress for the current decade.")]
    public async Task GetDecade()
    {
        var start = new DateTime(now.Year - (now.Year % 10), 1, 1);
        return ProgressResult(start, start.AddYears(10), $"{IrcValues.BOLD}The {start.Year}s{IrcValues.RESET} are");
    }

    [SlashCommand("yato-week", "Get progress for the current week starting on Monday, as the Japanese gods intended.")]
    public async Task GetOffsetWeek()
    {
        var start = now.StartOfWeek(DayOfWeek.Monday);
        return ProgressResult(start, start.AddDays(7), $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
    }

    [SlashCommand("week", "Get progress for the current week.")]
    public async Task GetWeek()
    {
        var start = now.StartOfWeek(DayOfWeek.Sunday);
        return ProgressResult(start, start.AddDays(7), $"{IrcValues.BOLD}This week{IrcValues.RESET} is");
    }

    [SlashCommand("century", "Get progress for the current century.")]
    public async Task GetCentury()
    {
        var start = new DateTime(now.Year - (now.Year % 100), 1, 1);
        var century = (now.Year / 100) + 1;
        return ProgressResult(start, start.AddYears(100), $"{IrcValues.BOLD}The {century.Ordinalize(ChatBeetConfiguration.Culture)} century{IrcValues.RESET} is");
    }

    [SlashCommand("millennium", "Get progress for the current millennium.")]
    public async Task GetMillennium()
    {
        var start = new DateTime(now.Year - (now.Year % 1000), 1, 1);
        return ProgressResult(start, start.AddYears(1000), $"{IrcValues.BOLD}This millennium{IrcValues.RESET} is");
    }

    [SlashCommand("second", "Get progress for the current millennium.")]
    public async Task GetSecond() =>
        new PrivateMessage(IncomingMessage.GetResponseTarget(), Progress.GetDescription((double)now.Millisecond / 1000, $"{IrcValues.BOLD}This second{IrcValues.RESET} is"));

    [SlashCommand("president", "Get progress for the current US presidential term.")]
    public async Task GetPresidentialTerm()
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

    private DiscordInteractionResponseBuilder ProgressResult(DateTime start, DateTime end, string preFormat)
    {

        new PrivateMessage(IncomingMessage.GetResponseTarget(), Progress.GetDescription(now, start, end, preFormat));
    }

    [SlashCommand("workday", "Get progress for your current workday.")]
    public async Task GetWorkday()
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
                var bar = Progress.GetDescription(now, start, end, $"{IrcValues.BOLD}Your workday{IrcValues.RESET} is");
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
