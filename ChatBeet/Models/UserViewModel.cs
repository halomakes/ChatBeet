using System.Collections.Generic;
using ChatBeet.Data.Entities;
using DSharpPlus.Entities;

namespace ChatBeet.Models;

public record UserViewModel(User User, IEnumerable<GuildViewModel> Guilds, string? AvatarUrl);

public record GuildViewModel(ulong Id, string? IconUrl, string? SplashUrl, string Name)
{
    public GuildViewModel(DiscordGuild guild) : this(guild.Id, guild.IconUrl, guild.SplashUrl, guild.Name)
    {
    }
}