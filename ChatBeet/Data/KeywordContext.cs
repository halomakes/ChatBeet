using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class KeywordContext : DbContext
    {
        public KeywordContext() : base() { }

        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<KeywordRecord> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db/keywords.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Keyword>().HasMany(k => k.Records).WithOne(r => r.Keyword);
        }
    }
}
