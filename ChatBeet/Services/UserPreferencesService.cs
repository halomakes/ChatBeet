using ChatBeet.Data;
using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class UserPreferencesService
    {
        private static bool Initialized;
        private readonly PreferencesContext db;

        public UserPreferencesService(PreferencesContext db)
        {
            this.db = db;
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

        public async Task<string> Get(string nick, UserPreference preference)
        {
            var pref = await db.PreferenceSettings.AsQueryable().FirstOrDefaultAsync(p => p.Nick == nick && p.Preference == preference);
            return pref?.Value;
        }
    }
}
