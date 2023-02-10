using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IDefinitionsRepository : IApplicationRepository
{
    DbSet<Definition> Definitions { get; }
}

public partial class CbDbContext : IDefinitionsRepository
{
    public virtual DbSet<Definition> Definitions { get; set; } = null!;

    private void ConfigureDefinitions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Definition>(builder =>
        {
            builder.ToTable("definitions", "interactions");
            builder.HasKey(b => new { b.Key, b.GuildId });
            builder.Property(b => b.Key)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(b => b.Value)
                .IsRequired()
                .HasMaxLength(3000);
            builder.HasOne(b => b.Guild)
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.CreatedBy)
                .HasPrincipalKey(b => b.Id);
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.Property(b => b.UpdatedAt)
                .HasDefaultValueSql("current_timestamp");
        });
    }
}