﻿using System.IO;
using System.Threading.Tasks;
using ChatBeet.Attributes;
using ChatBeet.Commands.Autocomplete;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using TimeZoneConverter;

namespace ChatBeet.Commands;

[SlashCommandGroup("progress", "Commands related to progress bars")]
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class ProgressCommandModule : ApplicationCommandModule
{
    private readonly UserPreferencesService _preferences;
    private readonly IProgressRepository _dbContext;
    private readonly IUsersRepository _users;
    private readonly DateTime _now;

    public ProgressCommandModule(UserPreferencesService preferences, IProgressRepository dbContext, IUsersRepository users)
    {
        _preferences = preferences;
        _dbContext = dbContext;
        _users = users;
        _now = DateTime.Now;
    }

    [SlashCommand("custom", "Gets progress over a period of time")]
    public async Task GetCustomTime(InteractionContext ctx, [Option("time-unit", "Unit of time"), Autocomplete(typeof(TimeRangesAutocompleteProvider))] string timeUnit)
    {
        var unit = await _dbContext.Spans.FirstOrDefaultAsync(r => r.GuildId == ctx.Guild.Id && r.Key.ToLower() == timeUnit.Trim().ToLower());
        if (unit is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Enter a valid time unit."));
            return;
        }

        var now = DateTime.Now;
        if (now < unit.StartDate && !string.IsNullOrWhiteSpace(unit.BeforeRangeMessage))
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(unit.BeforeRangeMessage));
        else if (now > unit.EndDate && !string.IsNullOrWhiteSpace(unit.AfterRangeMessage))
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(unit.AfterRangeMessage));
        else
            await SendResult(ctx, Progress.GetRatio(now, unit.StartDate, unit.EndDate), Progress.FormatTemplate(now, unit.StartDate, unit.EndDate, unit.Template));
    }

    [SlashCommand("year", "Get progress for the current year")]
    public async Task GetYear(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year, 1, 1, 0, 0, 0);
        await ProgressResult(ctx, start, start.AddYears(1), $"{Formatter.Bold(_now.Year.ToString())} is");
    }

    [SlashCommand("day", "Get progress for the current day")]
    public async Task GetDay(InteractionContext ctx)
    {
        var start = DateTime.Today;
        await ProgressResult(ctx, start, start.AddDays(1), $"{Formatter.Bold("Today")} is");
    }

    [SlashCommand("yato-day", "Get progress for the current day, but one hour in the past - it's objectively better")]
    public async Task GetOffsetDay(InteractionContext ctx)
    {
        var offset = TimeZoneInfo.Local.BaseUtcOffset - TimeSpan.FromHours(1);
        var centralTimeZone = TimeZoneInfo.CreateCustomTimeZone("yato", offset, "Yato", "Yato");
        var nowCentral = TimeZoneInfo.ConvertTime(DateTime.UtcNow, centralTimeZone);
        var startOfDay = new DateTimeOffset(nowCentral.Year, nowCentral.Month, nowCentral.Day, 0, 0, 0, centralTimeZone.BaseUtcOffset);
        await ProgressResult(ctx, startOfDay.UtcDateTime, startOfDay.AddDays(1).UtcDateTime, $"(in the {Formatter.Italic("objectively better")} time zone) {Formatter.Bold("Today")} is", DateTime.UtcNow);
    }

    [SlashCommand("hour", "Get progress for the current hour")]
    public async Task GetHour(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, 0, 0);
        await ProgressResult(ctx, start, start.AddHours(1), $"{Formatter.Bold("This hour")} is");
    }

    [SlashCommand("minute", "Get progress for the current minute")]
    public async Task GetMinute(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year, _now.Month, _now.Day, _now.Hour, _now.Minute, 0);
        await ProgressResult(ctx, start, start.AddMinutes(1), $"{Formatter.Bold("This minute")} is");
    }

    [SlashCommand("month", "Get progress for the current month")]
    public async Task GetMonth(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year, _now.Month, 1);
        await ProgressResult(ctx, start, start.AddMonths(1), $"{Formatter.Bold($"{_now:MMMM}")} is");
    }

    [SlashCommand("decade", "Get progress for the current decade")]
    public async Task GetDecade(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year - _now.Year % 10, 1, 1);
        await ProgressResult(ctx, start, start.AddYears(10), $"{Formatter.Bold($"The {start.Year}s")} are");
    }

    [SlashCommand("yato-week", "Get progress for the current week starting on Monday, as the Japanese gods intended")]
    public async Task GetOffsetWeek(InteractionContext ctx)
    {
        var start = _now.StartOfWeek(DayOfWeek.Monday);
        await ProgressResult(ctx, start, start.AddDays(7), $"{Formatter.Bold("This week")} is");
    }

    [SlashCommand("week", "Get progress for the current week")]
    public async Task GetWeek(InteractionContext ctx)
    {
        var start = _now.StartOfWeek(DayOfWeek.Sunday);
        await ProgressResult(ctx, start, start.AddDays(7), $"{Formatter.Bold("This week")} is");
    }

    [SlashCommand("century", "Get progress for the current century")]
    public async Task GetCentury(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year - _now.Year % 100, 1, 1);
        var century = _now.Year / 100 + 1;
        await ProgressResult(ctx, start, start.AddYears(100), $"{Formatter.Bold($"The {century.Ordinalize(ChatBeetConfiguration.Culture)} century")} is");
    }

    [SlashCommand("millennium", "Get progress for the current millennium")]
    public async Task GetMillennium(InteractionContext ctx)
    {
        var start = new DateTime(_now.Year - _now.Year % 1000, 1, 1);
        await ProgressResult(ctx, start, start.AddYears(1000), $"{Formatter.Bold("This millenium")} is");
    }

    [SlashCommand("second", "Get progress for the current millennium")]
    public async Task GetSecond(InteractionContext ctx)
    {
        await ProgressResult(ctx, (double)_now.Millisecond / 1000, $"{Formatter.Bold("This second")} is");
    }

    [SlashCommand("president", "Get progress for the current US presidential term")]
    public async Task GetPresidentialTerm(InteractionContext ctx)
    {
        // inauguration is January 20 at noon eastern time every 4 years (year after leap year)
        var termYears = 4;
        var startYear = _now.Year - _now.Year % termYears + 1;
        var easternTimeZone = TZConvert.GetTimeZoneInfo("Eastern Standard Time");
        var inauguration = new DateTimeOffset(new DateTime(startYear, 1, 20, 12, 0, 0, DateTimeKind.Unspecified), easternTimeZone.BaseUtcOffset);
        var start = (inauguration > _now ? inauguration.AddYears(-1 * termYears) : inauguration).DateTime;
        var end = (inauguration > _now ? inauguration : inauguration.AddYears(termYears)).DateTime;
        await ProgressResult(ctx, start, end, $"{Formatter.Bold("This presidential term")} is");
    }

    private Task ProgressResult(InteractionContext ctx, double ratio, string preFormat) =>
        SendResult(ctx, ratio, Progress.GetCompletionDescription(ratio, preFormat));

    private Task ProgressResult(InteractionContext ctx, DateTime start, DateTime end, string preFormat, DateTime? now = null) =>
        SendResult(ctx, Progress.GetRatio(now ?? _now, start, end), Progress.GetCompletionDescription(now ?? _now, start, end, preFormat));

    private async Task SendResult(InteractionContext ctx, double ratio, string message)
    {
        const int width = 200;
        const int height = 24;

        using var image = new Image<Rgba32>(width, height);
        var bgColor = Color.FromRgba(8, 9, 15, 120);
        image.Mutate(x => x.Fill(bgColor));
        var bar = new Rectangle(0, 0, (int)(width * ratio), height);
        var fgColor = GetGradientColor(ratio);
        image.Mutate(x => x.Fill(fgColor, bar));

        using var ms = new MemoryStream();
        var encoder = new WebpEncoder();
        await image.SaveAsync(ms, encoder);
        ms.Position = 0;

        var path = $"progress-{Guid.NewGuid()}.webp";
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(message)
            .AddFile(path, ms));

        static Color GetGradientColor(double ratio)
        {
            byte[] startColor = { 245, 66, 66 };
            byte[] endColor = { 78, 173, 84 };
            return Color.FromRgb(Blend(0), Blend(1), Blend(2));

            byte Blend(int index)
            {
                var range = endColor[index] - startColor[index];
                var offset = (int)(range * ratio);
                return (byte)(startColor[index] + offset);
            }
        }
    }

    [SlashCommand("workday", "Get progress for your current workday")]
    public async Task GetWorkday(InteractionContext ctx)
    {
        var user = await _users.GetUserAsync(ctx.User);
        var startPref = await _preferences.Get(user.Id, UserPreference.WorkHoursStart);
        var endPref = await _preferences.Get(user.Id, UserPreference.WorkHoursEnd);

        if (!IsValidDate(startPref))
        {
            var description = UserPreference.WorkHoursStart.GetAttribute<ParameterAttribute>().DisplayName;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"No valid value set for preference {Formatter.Italic(description)}"));
        }
        else if (!IsValidDate(endPref))
        {
            var description = UserPreference.WorkHoursEnd.GetAttribute<ParameterAttribute>().DisplayName;
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"No valid value set for preference {Formatter.Italic(description)}"));
        }
        else
        {
            var now = DateTime.Now;
            var start = NormalizeTime(DateTime.Parse(startPref!), now);
            var end = NormalizeTime(DateTime.Parse(endPref!), now);

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
                await ProgressResult(ctx, start, end, $"{Formatter.Bold("Your workday")} is");
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You are outside of working hours."));
            }
        }

        static bool IsValidDate(string? val) => !string.IsNullOrEmpty(val) && DateTime.TryParse(val, out _);

        static DateTime NormalizeTime(DateTime date, DateTime baseline) =>
            new(baseline.Year, baseline.Month, baseline.Day, date.Hour, date.Minute, date.Second);
    }
}