using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data
{
    public class ReplacementContext : DbContext
    {
        public ReplacementContext() : base() { }

        public virtual DbSet<ReplacementSet> Sets { get; set; }
        public virtual DbSet<ReplacementMapping> Mappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReplacementMapping>().HasKey(m => new { m.SetId, m.Replacement });
            modelBuilder.Entity<ReplacementSet>().HasMany(s => s.Mappings).WithOne();
        }
    }
}
