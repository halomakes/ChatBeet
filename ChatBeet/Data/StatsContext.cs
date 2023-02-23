using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;
using User = ChatBeet.Data.Entities.User;

namespace ChatBeet.Data;

public interface IStatsRepository : IApplicationRepository
{
    DbSet<StatEvent> StatEvents { get; }
}

public partial class CbDbContext : IStatsRepository
{
    public virtual DbSet<StatEvent> StatEvents { get; set; } = null!;

    private void ConfigureStats(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StatEvent>(builder =>
        {
            builder.ToTable("events", "stats");
            builder.HasKey(b => b.Id);
            builder.HasOne<Guild>()
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(g => g.Id)
                .IsRequired();
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.TriggeringUserId)
                .HasPrincipalKey(u => u.Id)
                .IsRequired();
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.TargetedUserId)
                .HasPrincipalKey(u => u.Id);
            builder.Property(b => b.EventType)
                .IsRequired();
            builder.Property(b => b.OccurredAt)
                .HasDefaultValueSql("current_timestamp");
        });
    }
}