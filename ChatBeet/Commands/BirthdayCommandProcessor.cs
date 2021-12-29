using ChatBeet.Attributes;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class BirthdayCommandProcessor : CommandProcessor
    {
        private readonly PreferencesContext db;
        private readonly IMemoryCache cache;

        public BirthdayCommandProcessor(PreferencesContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        [Command("birthday {nick}", Description = "Check when a user's special day is.")]
        public async Task<IClientMessage> LookupBirthday([Nick] string nick)
        {
            if (!string.IsNullOrEmpty(nick))
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), await GetUserBirthday(nick));
            else
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), await GetUpcomingBirthdays());
        }

        private async Task<string> GetUserBirthday(string nick)
        {
            var prefs = db.PreferenceSettings
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
            var allPrefs = await cache.GetOrCreate("prefs:birthday:upcoming", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                return await db.PreferenceSettings
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
            var upcomingString = string.Join(", ", upcoming.Select(u => $"{u.Nick} on {IrcValues.BOLD}{u.Date:MMMM d}{IrcValues.RESET}"));
            return $"Upcoming birthdays: {upcomingString}";
        }

        private DateTime GetNormalized(DateTime d)
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, d.Month, d.Day, 0, 0, 0);
        }
    }
}
