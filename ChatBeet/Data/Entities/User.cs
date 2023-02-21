using System.Collections.Generic;

namespace ChatBeet.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    public DiscordIdentity? Discord { get; set; }
    public IrcIdentity? Irc { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string? Mention() => Discord?.Id is not null
        ? $"<@{Discord.Id}>"
        : Irc?.Nick;

    public string? DisplayName() => Discord?.Name is not null
        ? $"{Discord?.Name}#{Discord?.Discriminator}"
        : Irc?.Nick;

    public virtual ICollection<UserPreferenceSetting>? Preferences { get; set; }
}

public class DiscordIdentity
{
    public ulong? Id { get; set; }
    public string? Name { get; set; }
    public string? Discriminator { get; set; }
}

public class IrcIdentity
{
    public string? Nick { get; set; }
}