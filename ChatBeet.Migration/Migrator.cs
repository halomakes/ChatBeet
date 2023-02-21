using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Migration.Legacy;
using Microsoft.EntityFrameworkCore;
using TagHistory = ChatBeet.Data.Entities.TagHistory;
using UserPreferenceSetting = ChatBeet.Data.Entities.UserPreferenceSetting;

namespace ChatBeet.Migration;

public class Migrator
{
    private readonly ulong _guild;

    public Migrator(ulong guild)
    {
        _guild = guild;
    }

    public async Task Migrate()
    {
        var options = new DbContextOptionsBuilder<CbDbContext>().UseNpgsql("connection string here").UseSnakeCaseNamingConvention().Options;
        await using var newDb = new CbDbContext(options);
        await newDb.Database.MigrateAsync();

        await MigrateIrcLinks(newDb);
        await EnsureGuildExists(newDb);
        await MigrateBooru(newDb);
        await MigrateDefinitions(newDb);
        await MigratePreferences(newDb);
        await MigrateProgress(newDb);
        await MigrateSuspicion(newDb);
    }

    private async Task EnsureGuildExists(CbDbContext newDb)
    {
        var existingGuild = await newDb.Guilds.FirstOrDefaultAsync(g => g.Id == _guild);
        if (existingGuild is not null)
            return;
        var user = await GetOrCreateUser(newDb, "carrots");
        newDb.Guilds.Add(new Guild
        {
            Id = _guild,
            Label = "Dtella",
            AddedBy = user.Id,
            AddedAt = DateTime.Now
        });
        await newDb.SaveChangesAsync();
    }

    private async Task MigrateIrcLinks(CbDbContext ctx)
    {
        var options = new DbContextOptionsBuilder<IrcLinkContext>().UseSqlite("Data Source=db/ircmigration.db").Options;
        await using var legacy = new IrcLinkContext(options);
        var links = await legacy.Links.ToListAsync();
        foreach (var link in links)
        {
            try
            {
                ctx.Users.Add(new User()
                {
                    CreatedAt = DateTime.Now,
                    Discord = new DiscordIdentity
                    {
                        Discriminator = link.Discriminator,
                        Id = link.Id,
                        Name = link.Username
                    },
                    Irc = new IrcIdentity
                    {
                        Nick = link.Nick
                    },
                    UpdatedAt = DateTime.Now,
                    Id = Guid.NewGuid()
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save user {link.Nick} - {e.Message}");
                Console.ReadKey();
            }
        }
    }

    private async Task MigrateBooru(CbDbContext ctx)
    {
        var options = new DbContextOptionsBuilder<BooruContext>().UseSqlite("Data Source=db/booru.db").Options;
        await using var legacy = new BooruContext(options);
        var blacklists = await legacy.Blacklists.ToListAsync();
        foreach (var blacklist in blacklists)
        {
            try
            {
                var user = await GetOrCreateUser(ctx, blacklist.Nick);
                ctx.BlacklistedTags.Add(new BlacklistedTag()
                {
                    UserId = user.Id,
                    Tag = blacklist.Tag
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save blacklist {blacklist.Nick}:{blacklist.Tag} - {e.Message}");
                Console.ReadKey();
            }
        }

        var logs = await legacy.TagHistories.ToListAsync();
        foreach (var log in logs)
        {
            try
            {
                var user = await GetOrCreateUser(ctx, log.Nick);
                ctx.TagHistories.Add(new TagHistory()
                {
                    UserId = user.Id,
                    Tag = log.Tag,
                    CreatedAt = log.Timestamp,
                    Id = Guid.NewGuid()
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save tag hit {log.Nick}:{log.Tag} - {e.Message}");
                Console.ReadKey();
            }
        }
    }

    private async Task MigrateDefinitions(CbDbContext ctx)
    {
        var options = new DbContextOptionsBuilder<MemoryCellContext>().UseSqlite("Data Source=db/memorycell.db").Options;
        await using var legacy = new MemoryCellContext(options);
        var cells = await legacy.MemoryCells.ToListAsync();
        foreach (var cell in cells)
        {
            try
            {
                var user = await GetOrCreateUser(ctx, cell.Author);
                ctx.Definitions.Add(new Definition()
                {
                    CreatedAt = DateTime.Now,
                    GuildId = _guild,
                    Value = cell.Value,
                    Key = cell.Key,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = user.Id
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save definition {cell.Key} - {e.Message}");
                Console.ReadKey();
            }
        }
    }

    private async Task MigratePreferences(CbDbContext ctx)
    {
        var options = new DbContextOptionsBuilder<PreferencesContext>().UseSqlite("Data Source=db/userprefs.db").Options;
        await using var legacy = new PreferencesContext(options);
        var preferences = await legacy.PreferenceSettings.ToListAsync();
        foreach (var pref in preferences)
        {
            try
            {
                var user = await GetOrCreateUser(ctx, pref.Nick);
                ctx.UserPreferences.Add(new UserPreferenceSetting()
                {
                    UserId = user.Id,
                    Preference = pref.Preference,
                    Value = pref.Value
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save preference {pref.Nick}:{pref.Preference} - {e.Message}");
                Console.ReadKey();
            }
        }
    }

    private async Task MigrateProgress(CbDbContext ctx)
    {
        var options = new DbContextOptionsBuilder<ProgressContext>().UseSqlite("Data Source=db/progress.db").Options;
        await using var legacy = new ProgressContext(options);
        var spans = await legacy.FixedTimeRanges.ToListAsync();
        foreach (var span in spans)
        {
            try
            {
                ctx.Spans.Add(new ProgressSpan
                {
                    GuildId = _guild,
                    Template = span.Template,
                    BeforeRangeMessage = span.BeforeRangeMessage,
                    AfterRangeMessage = span.AfterRangeMessage,
                    Key = span.Key,
                    StartDate = span.StartDate,
                    EndDate = span.EndDate
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save span {span.Key} - {e.Message}");
                Console.ReadKey();
            }
        }
    }

    private async Task MigrateSuspicion(CbDbContext ctx)
    {
        var options = new DbContextOptionsBuilder<SuspicionContext>().UseSqlite("Data Source=db/suspicions.db").Options;
        await using var legacy = new SuspicionContext(options);
        var sussyBakas = await legacy.Suspicions.ToListAsync();
        foreach (var sus in sussyBakas)
        {
            try
            {
                var reporter = await GetOrCreateUser(ctx, sus.Reporter);
                var suspect = await GetOrCreateUser(ctx, sus.Suspect);
                ctx.Suspicions.Add(new SuspicionReport()
                {
                    Id = Guid.NewGuid(),
                    GuildId = _guild,
                    SuspectId = suspect.Id,
                    ReporterId = reporter.Id,
                    CreatedAt = sus.TimeReported
                });
                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to save suspicion {sus.Reporter}:{sus.Suspect} - {e.Message}");
                Console.ReadKey();
            }
        }
    }

    private async Task<User> GetOrCreateUser(CbDbContext ctx, string nick)
    {
        var existingUser = await ctx.Users.FirstOrDefaultAsync(u => u.Irc!.Nick!.ToLower() == nick.ToLower());
        if (existingUser is not null)
            return existingUser;
        var entry = ctx.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Irc = new IrcIdentity
            {
                Nick = nick
            },
            Discord = new(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await ctx.SaveChangesAsync();
        return entry.Entity;
    }
}