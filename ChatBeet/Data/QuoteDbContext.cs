using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IQuoteRepository : IApplicationRepository
{
    DbSet<Quote> Quotes { get; }
}

public partial class CbDbContext : IQuoteRepository
{
    public virtual DbSet<Quote> Quotes { get; set; } = null!;

    private void ConfigureQuotes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quote>(builder =>
        {
            builder.ToTable("quotes", "interactions");
            builder.HasKey(b => new { b.GuildId, b.Slug });
            builder.HasOne<Guild>()
                .WithMany()
                .HasForeignKey(b => b.GuildId)
                .HasPrincipalKey(b => b.Id);
            builder.HasOne(b => b.SavedBy)
                .WithMany()
                .HasForeignKey(b => b.SavedById)
                .HasPrincipalKey(b => b.Id)
                .IsRequired();
            builder.Property(b => b.ChannelName)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(b => b.Slug)
                .IsRequired()
                .HasMaxLength(200);
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.OwnsMany(b => b.Messages, b =>
            {
                b.HasOne(m => m.Author)
                    .WithMany()
                    .HasForeignKey(m => m.AuthorId)
                    .HasPrincipalKey(u => u.Id);
            });
            builder.Navigation(b => b.Messages)
                .AutoInclude(false);
        });
    }
}