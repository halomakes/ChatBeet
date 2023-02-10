using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Data;

public interface IBooruRepository : IApplicationRepository
{
    DbSet<BlacklistedTag> BlacklistedTags { get; }
    DbSet<TagHistory> TagHistories { get; }
    Task<List<TopTag>> GetTopTags();
}

public partial class CbDbContext : IBooruRepository
{
    public virtual DbSet<BlacklistedTag> BlacklistedTags { get; set; } = null!;
    public virtual DbSet<TagHistory> TagHistories { get; set; } = null!;
    public virtual DbSet<TopTag> TopTags { get; set; } = null!;

    public Task<List<TopTag>> GetTopTags() => TopTags
        .FromSqlRaw(@"select Tag, Nick, Total from (select t.Id, t.Tag, t.Nick, count(*) as Total from TagHistories t group by t.Tag, t.Nick order by Total desc) i group by i.Nick order by i.Total desc limit 10")
        .ToListAsync();

    private void ConfigureBooru(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlacklistedTag>(builder =>
        {
            builder.ToTable("blacklisted_tags", "booru");
            builder.HasKey(b => new { b.UserId, b.Tag });
            builder.Property(b => b.Tag)
                .IsRequired()
                .HasMaxLength(150);
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);
        });
        modelBuilder.Entity<TagHistory>(builder =>
        {
            builder.ToTable("tag_history", "booru");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.UserId)
                .IsRequired();
            builder.Property(b => b.Tag)
                .IsRequired()
                .HasMaxLength(150);
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId);
        });
        modelBuilder.Entity<TopTag>().HasNoKey();
    }
}