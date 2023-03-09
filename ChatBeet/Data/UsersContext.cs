using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Data;

public interface IUsersRepository : IApplicationRepository
{
    DbSet<User> Users { get; }
    DbSet<Guild> Guilds { get; }
    DbSet<UserPreferenceSetting> UserPreferences { get; }
    DbSet<UserMetadata> Metadata { get; }
    Task<User> GetUserAsync(string ircNick);
    Task<User> GetUserAsync(DiscordUser user, CancellationToken cancellationToken = default);
}

public partial class CbDbContext : IUsersRepository
{
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<Guild> Guilds { get; set; } = null!;
    public virtual DbSet<UserPreferenceSetting> UserPreferences { get; set; } = null!;
    public virtual DbSet<UserMetadata> Metadata { get; set; } = null!;

    public async Task<User> GetUserAsync(DiscordUser user, CancellationToken cancellationToken = default)
    {
        var internalUser = await Users.FirstOrDefaultAsync(l => l.Discord!.Id == user.Id, cancellationToken);
        return internalUser ?? await CreateUser(user, cancellationToken);
    }

    public async Task<User> GetUserAsync(string ircNick)
    {
        var (success, _, _) = ircNick.ParseUsername();
        if (!success)
            return await Users.FirstOrDefaultAsync(u => u.Irc!.Nick!.ToLower() == ircNick.ToLower()) ?? throw new ArgumentException("Pass in the DiscordUser please");
        throw new ArgumentException("Pass in the DiscordUser please");
    }

    private async Task<User> CreateUser(DiscordUser user, CancellationToken cancellationToken = default)
    {
        var created = Users.Add(new()
        {
            Id = Guid.NewGuid(),
            Discord = new()
            {
                Id = user.Id,
                Discriminator = user.Discriminator,
                Name = user.Username
            }
        });
        await SaveChangesAsync(cancellationToken);
        return created.Entity;
    }

    private void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("users", "core");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.Property(b => b.UpdatedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.OwnsOne(b => b.Discord, owned =>
            {
                owned.Property(o => o.Discriminator)
                    .HasMaxLength(10);
                owned.Property(o => o.Name)
                    .HasMaxLength(200);
            });
            builder.OwnsOne(b => b.Irc, owned =>
            {
                owned.Property(o => o.Nick)
                    .HasMaxLength(100);
            });
            builder.HasMany(b => b.Preferences)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .HasPrincipalKey(b => b.Id);
        });
        modelBuilder.Entity<UserPreferenceSetting>(builder =>
        {
            builder.ToTable("user_preferences", "core");
            builder.HasKey(b => new { b.UserId, b.Preference });
            builder.Property(b => b.Value)
                .IsRequired()
                .HasMaxLength(300);
        });
        modelBuilder.Entity<Guild>(builder =>
        {
            builder.ToTable("guilds", "core");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Label)
                .IsRequired()
                .HasMaxLength(150);
            builder.Property(b => b.AddedAt)
                .HasDefaultValueSql("current_timestamp");
            builder.HasOne(b => b.AddedByUser)
                .WithMany()
                .HasForeignKey(b => b.AddedBy)
                .HasPrincipalKey(b => b.Id);
        });
        modelBuilder.Entity<UserMetadata>(builder =>
        {
            builder.ToTable("user_metadata", "core");
            builder.HasKey(u => new { u.UserId, u.GuildId, u.Key });
            builder.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .HasPrincipalKey(u => u.Id);
            builder.HasOne<Guild>()
                .WithMany()
                .HasForeignKey(m => m.GuildId)
                .HasPrincipalKey(g => g.Id);
            builder.Property(b => b.Key)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(b => b.Value)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(b => b.AuthorId)
                .IsRequired();
            builder.HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .HasPrincipalKey(b => b.Id);
        });
    }
}