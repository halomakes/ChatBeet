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
    private readonly IUsersRepository _db;
    private readonly IMemoryCache _cache;

    public BirthdayCommandModule(IUsersRepository db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    [SlashCommand("birthday", "Check when a user's special day is")]
    public async Task LookupBirthday(InteractionContext ctx, [Option("user", "User to look up the birthday for")] DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(await GetUserBirthday(user))
        );
    }

    [SlashCommand("upcoming-birthdays", "See a list of upcoming birthdays")]
    public async Task GetUpcomingBirthdays(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(await GetUpcomingBirthdays())
        );
    }

    private async Task<string> GetUserBirthday(DiscordUser discordUser)
    {
        var user = await _db.Users
            .Include(u => u.Preferences)
            .FirstAsync(u => u.Discord!.Id == discordUser.Id);
        var birthdayPref = user.Preferences!
            .Where(s => s.Preference == UserPreference.DateOfBirth)
            .Select(s => s.Value)
            .FirstOrDefault();
        var pronounPref = user.Preferences!
            .Where(s => s.Preference == UserPreference.PossessivePronoun)
            .Select(s => s.Value)
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(birthdayPref) && DateTime.TryParse(birthdayPref, out var d))
        {
            var possessive = string.IsNullOrEmpty(pronounPref) ? "Their" : pronounPref.CapitalizeFirst();
            return $"{possessive} birthday is on {d:MMMM d}";
        }
        else
        {
            return $"I don't know the birthday for {Formatter.Mention(discordUser)}";
        }
    }

    private async Task<string> GetUpcomingBirthdays()
    {
        var allPrefs = await _cache.GetOrCreate("prefs:birthday:upcoming", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            return await _db.UserPreferences
                .Include(p => p.User)
                .Where(s => s.Preference == UserPreference.DateOfBirth)
                .ToListAsync();
        })!;

        var today = GetNormalized(DateTime.Now);
        var dateMappings = allPrefs
            .Where(p => p.Preference == UserPreference.DateOfBirth)
            .Where(p => DateTime.TryParse(p.Value, out _))
            .Select(p => (Date: GetNormalized(DateTime.Parse(p.Value)), p.User))
            .ToList();
        var doubleYear = dateMappings.Union(dateMappings.Select(m => (Date: m.Date.AddYears(1), m.User)));
        var upcoming = doubleYear.Where(m => m.Date >= today).OrderBy(m => m.Date).Take(5).DistinctBy(m => m.User?.Id);
        var upcomingString = string.Join(Environment.NewLine, upcoming.Select(u => $"{u.User?.Mention()} on {Formatter.Bold($"{u.Date}:MMMM d")}"));
        return $"Upcoming birthdays: {upcomingString}";
    }

    private DateTime GetNormalized(DateTime d)
    {
        var now = DateTime.Now;
        return new DateTime(now.Year, d.Month, d.Day, 0, 0, 0);
    }
}