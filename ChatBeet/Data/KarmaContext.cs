using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IKarmaRepository : IApplicationRepository
{
    DbSet<Karma> Karma { get; }
}

public partial class CbDbContext : IKarmaRepository
{
    public virtual DbSet<Karma> Karma { get; set; } = null!;

    private void ConfigureKarma(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Karma>(builder =>
        {
            builder.ToTable("karma", "interactions");
            builder.HasKey(b => new { b.GuildId, b.Key });
            builder.HasOne(b => b.Guild)
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.Property(b => b.Key)
                .IsRequired()
                .HasMaxLength(200);
        });
    }
}