using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IKarmaRepository : IApplicationRepository
{
    DbSet<KarmaVote> Karma { get; }
}

public partial class CbDbContext : IKarmaRepository
{
    public virtual DbSet<KarmaVote> Karma { get; set; } = null!;

    private void ConfigureKarma(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KarmaVote>(builder =>
        {
            builder.ToTable("karma", "interactions");
            builder.HasKey(b => b.Id);
            builder.HasOne(b => b.Guild)
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.Property(b => b.Key)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasOne(b => b.Voter)
                .WithMany()
                .HasForeignKey(b => b.VoterId)
                .HasPrincipalKey(b => b.Id);
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("current_timestamp");
        });
    }
}