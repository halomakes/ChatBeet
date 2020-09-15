using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class PreferencesContext : DbContext
    {
        public PreferencesContext() : base() { }

        public virtual DbSet<UserPreferenceSetting> PreferenceSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserPreferenceSetting>().HasKey(s => new { s.Nick, s.Preference });
        }
    }
}
