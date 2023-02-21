using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IKeywordsRepository : IApplicationRepository
{
    DbSet<Keyword> Keywords { get; }
    DbSet<KeywordHit> Hits { get; }
}

public partial class CbDbContext : IKeywordsRepository
{
    public virtual DbSet<Keyword> Keywords { get; set; } = null!;
    public virtual DbSet<KeywordHit> Hits { get; set; } = null!;

    private void ConfigureKeywords(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Keyword>(builder =>
        {
            builder.ToTable("keywords", "stats");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Label)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(b => b.Regex)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasOne(b => b.Guild)
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.HasMany(b => b.Hits)
                .WithOne()
                .HasForeignKey(b => b.KeywordId)
                .HasPrincipalKey(b => b.Id);
        });
        modelBuilder.Entity<KeywordHit>(builder =>
        {
            builder.ToTable("keyword_hits", "stats");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Message)
                .IsRequired();
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .HasPrincipalKey(b => b.Id);
        });
    }
}