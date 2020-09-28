using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class BirthdaysRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly PreferencesContext db;
        private readonly Regex rgx;
        private readonly IMemoryCache cache;

        public BirthdaysRule(PreferencesContext db, IOptions<IrcBotConfiguration> opts, IMemoryCache cache)
        {
            this.db = db;
            config = opts.Value;
            this.cache = cache;
            rgx = new Regex($@"(?:^(?:{Regex.Escape(config.Nick)},? (?:when)(?: is|['ʼ]s)? ({RegexUtils.Nick})(?:['ʼ]s?)? birthday\??))|(?:^{Regex.Escape(config.CommandPrefix)}birthday( {RegexUtils.Nick})?)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var naturalGroup = match.Groups[1].Value?.Trim();
                var commandGroup = match.Groups[2].Value?.Trim();
                var nick = string.IsNullOrEmpty(naturalGroup) ? commandGroup : naturalGroup;
                if (!string.IsNullOrEmpty(nick))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), await GetUserBirthday(nick));
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), await GetUpcomingBirthdays());
                }
            }
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
