using System.ComponentModel.DataAnnotations;
using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Migration.Legacy;

public class BooruBlacklist
{
    public string Nick { get; set; }

    public string Tag { get; set; }
}
public class FixedTimeRange
{
    [Key]
    public string Key { get; set; }
    public string Template { get; set; } = "This custom range is {percentage} complete.";
    public string? BeforeRangeMessage { get; set; }
    public string? AfterRangeMessage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
public class IrcLink
{
    public ulong Id { get; set; }
    public string Username { get; set; }
    public string Discriminator { get; set; }
    public string Nick { get; set; }
}
public class MemoryCell
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public string Author { get; set; }
}
public class Suspicion
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime TimeReported { get; set; }

    [Required]
    public string Reporter { get; set; }

    [Required]
    public string Suspect { get; set; }
}
public class TagHistory
{
    [Key]
    public int Id { get; set; }
    public string Nick { get; set; }
    public string Tag { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
public class TopTag
{
    public string Nick { get; set; }
    public string Tag { get; set; }
    public int Total { get; set; }
}
public class UserPreferenceSetting
{
    public string Nick { get; set; }
    public UserPreference Preference { get; set; }
    public string Value { get; set; }
}
public class BooruContext : DbContext
{
    public BooruContext(DbContextOptions<BooruContext> optsBuilder) : base(optsBuilder) { }

    public virtual DbSet<BooruBlacklist> Blacklists { get; set; }
    public virtual DbSet<TagHistory> TagHistories { get; set; }
    public virtual DbSet<TopTag> TopTags { get; set; }

    public Task<List<TopTag>> GetTopTags() => TopTags
        .FromSqlRaw(@"select Tag, Nick, Total from (select t.Id, t.Tag, t.Nick, count(*) as Total from TagHistories t group by t.Tag, t.Nick order by Total desc) i group by i.Nick order by i.Total desc limit 10")
        .ToListAsync();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BooruBlacklist>().HasKey(b => new { b.Nick, b.Tag });
        modelBuilder.Entity<TopTag>().HasNoKey();
    }
}
public class IrcLinkContext : DbContext
{
    public IrcLinkContext(DbContextOptions<IrcLinkContext> optsBuilder) : base(optsBuilder) { }

    public virtual DbSet<IrcLink> Links { get; set; }
}
public class MemoryCellContext : DbContext
{
    public MemoryCellContext(DbContextOptions<MemoryCellContext> optsBuilder) : base(optsBuilder) { }

    public virtual DbSet<MemoryCell> MemoryCells { get; set; }
}
public class PreferencesContext : DbContext
{
    public PreferencesContext(DbContextOptions<PreferencesContext> optsBuilder) : base(optsBuilder) { }

    public virtual DbSet<UserPreferenceSetting> PreferenceSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPreferenceSetting>().HasKey(s => new { s.Nick, s.Preference });
    }
}
public class ProgressContext : DbContext
{
    public ProgressContext(DbContextOptions<ProgressContext> optsBuilder) : base(optsBuilder) { }

    public virtual DbSet<FixedTimeRange> FixedTimeRanges { get; set; }
}
public class SuspicionContext : DbContext
{
    public SuspicionContext(DbContextOptions<SuspicionContext> opts) : base(opts)
    {
    }

    public virtual DbSet<Suspicion> Suspicions { get; set; }
}