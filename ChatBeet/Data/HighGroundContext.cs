using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IHighGroundRepository : IApplicationRepository
{
    DbSet<HighGround> Claims { get; }
}

public partial class CbDbContext : IHighGroundRepository
{
    public virtual DbSet<HighGround> Claims { get; set; } = null!;

    private void ConfigureHighGround(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HighGround>(builder =>
        {
            builder.ToTable("high_ground", "interactions");
            builder.HasKey(b => b.GuildId);
            builder.HasOne(b => b.Guild)
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .HasPrincipalKey(b => b.Id);
            builder.Property(b => b.UpdatedAt)
                .HasDefaultValueSql("current_timestamp");
        });
    }
}