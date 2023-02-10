using System;
using System.Linq;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BirthdayCommandModule : ApplicationCommandModule
{
    private readonly PreferencesContext _db;
    private readonly IMemoryCache _cache;

    public BirthdayCommandModule(PreferencesContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    [SlashCommand("birthday", "Check when a user's special day is")]
    public async Task LookupBirthday(InteractionContext ctx, [Option("user", "User to look up the birthday for")] DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(await GetUserBirthday(user.Username))
                );
    }

    [SlashCommand("upcoming-birthdays", "See a list of upcoming birthdays")]
    public async Task GetUpcomingBirthdays(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(await GetUpcomingBirthdays())
                );
    }

    private async Task<string> GetUserBirthday(string nick)
    {
        var prefs = _db.PreferenceSettings
            .AsQueryable()
            .Where(s => s.Nick.ToLower() == nick.ToLower());
        var birthdayPref = await prefs
            .Where(s => s.Preference == UserPreference.DateOfBirth)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();
        var pronounPref = await prefs
            .Where(s => s.Preference == UserPreference.PossessivePronoun)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrEmpty(birthdayPref) && DateTime.TryParse(birthdayPref, out var d))
        {
            var possessive = string.IsNullOrEmpty(pronounPref) ? "Their" : pronounPref.CapitalizeFirst();
            return $"{possessive} birthday is on {d:MMMM d}";
        }
        else
        {
            return $"I don't know the birthday for {nick}";
        }
    }

    private async Task<string> GetUpcomingBirthdays()
    {
        var allPrefs = await _cache.GetOrCreate("prefs:birthday:upcoming", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            return await _db.PreferenceSettings
                .AsQueryable()
                .Where(s => s.Preference == UserPreference.DateOfBirth)
                .ToListAsync();
        });

        var today = GetNormalized(DateTime.Now);
        var dateMappings = allPrefs
            .Where(p => p.Preference == UserPreference.DateOfBirth)
            .Where(p => DateTime.TryParse(p.Value, out var d))
            .Select(p => (Date: GetNormalized(DateTime.Parse(p.Value)), p.Nick));
        var doubleYear = dateMappings.Union(dateMappings.Select(m => (Date: m.Date.AddYears(1), m.Nick)));
        var upcoming = doubleYear.Where(m => m.Date >= today).OrderBy(m => m.Date).Take(5).DistinctBy(m => m.Nick);
        var upcomingString = string.Join(Environment.NewLine, upcoming.Select(u => $"{u.Nick} on {Formatter.Bold($"{u.Date}:MMMM d")}"));
        return $"Upcoming birthdays: {upcomingString}";
    }

    private DateTime GetNormalized(DateTime d)
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, d.Month, d.Day, 0, 0, 0);
    }
}