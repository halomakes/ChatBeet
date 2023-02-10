using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface ISuspicionRepository : IApplicationRepository
{
    DbSet<SuspicionReport> Suspicions { get; }
}

public partial class CbDbContext : ISuspicionRepository
{
    public virtual DbSet<SuspicionReport> Suspicions { get; set; } = null!;

    private void ConfigureSuspicion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SuspicionReport>(builder =>
        {
            builder.ToTable("suspicion_report", "interactions");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.HasOne(b => b.Guild)
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.HasOne(b => b.Reporter)
                .WithMany()
                .HasForeignKey(b => b.ReporterId)
                .HasPrincipalKey(b => b.Id);
            builder.HasOne(b => b.Suspect)
                .WithMany()
                .HasForeignKey(b => b.SuspectId)
                .HasPrincipalKey(b => b.Id);
        });
    }
}