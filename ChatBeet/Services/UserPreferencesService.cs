using ChatBeet.Attributes;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class UserPreferencesService
    {
        private static bool Initialized;
        private readonly PreferencesContext db;
        private readonly ChatBeetConfiguration config;

        public UserPreferencesService(PreferencesContext db, IOptions<ChatBeetConfiguration> opts)
        {
            this.db = db;
            config = opts.Value;
            if (!Initialized)
            {
                db.Database.EnsureCreated();
                Initialized = true;
            }
        }

        public async Task Set(string nick, UserPreference preference, string value)
        {
            var existingPref = await db.PreferenceSettings.AsQueryable().FirstOrDefaultAsync(p => p.Nick == nick && p.Preference == preference);
            if (existingPref == null)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    db.PreferenceSettings.Add(new UserPreferenceSetting
                    {
                        Nick = nick,
                        Preference = preference,
                        Value = value
                    });
                }
            }
            else
            {
                if (string.IsNullOrEmpty(value))
                {
                    db.PreferenceSettings.Remove(existingPref);
                }
                else
                {
                    existingPref.Value = value.ToString();
                }
            }

            await db.SaveChangesAsync();
        }

        public Task Set(PreferenceChange change) => Set(change.Nick, change.Preference, change.Value);

        public async Task<string> Get(string nick, UserPreference preference)
        {
            var pref = await db.PreferenceSettings.AsQueryable().FirstOrDefaultAsync(p => p.Nick == nick && p.Preference == preference);
            return pref?.Value;
        }

        public Task<List<UserPreferenceSetting>> Get(string nick) => db.PreferenceSettings.AsQueryable().Where(p => p.Nick == nick).ToListAsync();

        public string GetValidation(UserPreference preference, string value)
        {
            var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;
            return string.IsNullOrEmpty(value) ? default : preference switch
            {
                UserPreference.SubjectPronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Subjects, displayName),
                UserPreference.ObjectPronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Objects, displayName),
                UserPreference.PossessivePronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Possessives, displayName),
                UserPreference.ReflexivePronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Reflexives, displayName),
                UserPreference.DateOfBirth => GetDateValidation(value),
                UserPreference.WorkHoursEnd => GetDateValidation(value),
                UserPreference.WorkHoursStart => GetDateValidation(value),
                _ => default
            };
        }

        private static string GetCollectionValidation(string value, IEnumerable<string> collection, string displayName)
        {
            if (!collection.Contains(value.ToLower()))
            {
                return $"Sorry, {value} is not an available value for {displayName}.  Available values are [{string.Join(", ", collection)}].";
            }
            return default;
        }

        private static string GetDateValidation(string value)
        {
            if (!DateTime.TryParse(value, out var _))
            {
                return $"{value} is not a valid date.";
            }
            return default;
        }
    }
}
