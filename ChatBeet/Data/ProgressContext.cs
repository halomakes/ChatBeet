using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IProgressRepository : IApplicationRepository
{
    DbSet<ProgressSpan> Spans { get; }
}

public partial class CbDbContext : IProgressRepository
{
    public virtual DbSet<ProgressSpan> Spans { get; set; } = null!;

    private void ConfigureProgress(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgressSpan>(builder =>
        {
            builder.ToTable("progress_spans", "interactions");
            builder.HasKey(b => new { b.Key, b.GuildId });
            builder.Property(b => b.Key)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(b => b.Template)
                .IsRequired()
                .HasMaxLength(300);
            builder.Property(b => b.BeforeRangeMessage)
                .HasMaxLength(300);
            builder.Property(b => b.AfterRangeMessage)
                .HasMaxLength(300);
        });
    }
}