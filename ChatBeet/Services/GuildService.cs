using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ChatBeet.Services;

public class GuildService
{
    private readonly DiscordClient _discord;
    private readonly IMemoryCache _cache;

    public GuildService(DiscordClient discord, IMemoryCache cache)
    {
        _discord = discord;
        _cache = cache;
    }

    public IEnumerable<DiscordGuild> GetGuilds() => _discord.Guilds.Values;

    public async Task<IEnumerable<DiscordMember>> GetMembersAsync(DiscordGuild guild) => (await _cache.GetOrCreateAsync($"guild:{guild.Id}:members", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return (await guild.GetAllMembersAsync()).ToList();
    }))!;
}