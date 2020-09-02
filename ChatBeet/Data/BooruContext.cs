using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class BooruContext : DbContext
    {
        public BooruContext() : base() { }

        public virtual DbSet<BooruBlacklist> Blacklists { get; set; }
        public virtual DbSet<TagHistory> TagHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db/booru.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BooruBlacklist>().HasKey(b => new { b.Nick, b.Tag });
        }
    }
}
