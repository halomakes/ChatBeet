using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Data
{
    public class BooruContext : DbContext
    {
        public BooruContext() : base() { }

        public virtual DbSet<BooruBlacklist> Blacklists { get; set; }
        public virtual DbSet<TagHistory> TagHistories { get; set; }
        public virtual DbSet<TopTag> TopTags { get; set; }

        public Task<List<TopTag>> GetTopTags() => TopTags
            .FromSqlRaw(@"select Tag, Nick, Total from (select t.Id, t.Tag, t.Nick, count(*) as Total from TagHistories t group by t.Tag, t.Nick order by Total desc) i group by i.Nick order by i.Total desc limit 10")
            .ToListAsync();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BooruBlacklist>().HasKey(b => new { b.Nick, b.Tag });
            modelBuilder.Entity<TopTag>().HasNoKey();
        }
    }
}
