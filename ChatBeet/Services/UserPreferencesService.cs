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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnitsNet.Units;

namespace ChatBeet.Services
{
    public class UserPreferencesService
    {
        private readonly PreferencesContext db;
        private readonly ChatBeetConfiguration config;

        public UserPreferencesService(PreferencesContext db, IOptions<ChatBeetConfiguration> opts)
        {
            this.db = db;
            config = opts.Value;
        }

        public async Task<string> Set(string nick, UserPreference preference, string value)
        {
            var isDelete = string.IsNullOrEmpty(value);
            var normalized = isDelete ? default : Normalize(preference, value);

            var existingPref = await db.PreferenceSettings.AsQueryable().FirstOrDefaultAsync(p => p.Nick == nick && p.Preference == preference);
            if (existingPref == default)
            {
                if (!isDelete)
                {
                    db.PreferenceSettings.Add(new UserPreferenceSetting
                    {
                        Nick = nick,
                        Preference = preference,
                        Value = normalized
                    });
                }
            }
            else
            {
                if (isDelete)
                {
                    db.PreferenceSettings.Remove(existingPref);
                }
                else
                {
                    existingPref.Value = normalized;
                }
            }

            await db.SaveChangesAsync();

            return normalized;
        }

        public Task<string> Set(PreferenceChange change) => Set(change.Nick, change.Preference.Value, change.Value);

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
                UserPreference.WeatherLocation => GetZipValidation(value),
                UserPreference.WeatherTempUnit => GetEnumValidation<TemperatureUnit>(value, displayName),
                UserPreference.WeatherPrecipUnit => GetEnumValidation<LengthUnit>(value, displayName),
                UserPreference.WeatherWindUnit => GetEnumValidation<SpeedUnit>(value, displayName),
                _ => default
            };
        }

        public string Normalize(UserPreference preference, string value) => preference switch
        {
            UserPreference.DateOfBirth => GetNormalizedDayOfYear(value),
            UserPreference.WorkHoursEnd => GetNormalizedTimeOfDay(value),
            UserPreference.WorkHoursStart => GetNormalizedTimeOfDay(value),
            UserPreference.WeatherTempUnit => GetNormalizedEnum<TemperatureUnit>(value),
            UserPreference.WeatherPrecipUnit => GetNormalizedEnum<LengthUnit>(value),
            UserPreference.WeatherWindUnit => GetNormalizedEnum<SpeedUnit>(value),
            _ => value
        };

        private static string GetNormalizedDayOfYear(string value) => DateTime.Parse(value).ToString("MMMM dd");

        private static string GetNormalizedTimeOfDay(string value) => DateTime.Parse(value).ToString("HH:mm:sszzz");

        private static string GetNormalizedEnum<TEnum>(string value) where TEnum : struct, Enum => Enum.Parse<TEnum>(value).ToString();

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

        private static string GetZipValidation(string value)
        {
            var rgx = new Regex(@"(\d{5}(?:-\d{4})?)");
            if (!rgx.IsMatch(value))
                return $"{value} is not a valid ZIP code.";
            return default;
        }

        private static string GetEnumValidation<TEnum>(string value, string displayName) where TEnum : struct, Enum
        {
            if (!Enum.TryParse<TEnum>(value, out var _))
            {
                return $"{value} is not an available value for {displayName}.  Available values are [{string.Join(", ", Enum.GetNames(typeof(TEnum)))}].";
            }
            return default;
        }
    }
}
