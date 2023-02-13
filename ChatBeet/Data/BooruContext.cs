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
        .FromSql($"select max(tag) as tag, user_id, max(total) as total from (select t.tag, t.user_id, count(*) as total from booru.tag_history t group by t.tag, t.user_id order by total desc) i group by i.user_id order by max(i.total) desc limit 10")
        .Include(t => t.User)
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
        modelBuilder.Entity<TopTag>(builder =>
        {
            builder.HasNoKey();
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .HasPrincipalKey(u => u.Id);
        });
    }
}