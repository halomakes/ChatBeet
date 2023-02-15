using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Services;

public class KarmaService
{
    private static readonly TimeSpan RateLimit = TimeSpan.FromMinutes(2);
    private readonly IKarmaRepository _karma;
    private readonly IUsersRepository _users;
    private readonly GuildService _guilds;

    public KarmaService(IKarmaRepository karma, IUsersRepository users, GuildService guilds)
    {
        _karma = karma;
        _users = users;
        _guilds = guilds;
    }

    public async Task IncrementAsync(ulong guildId, string target, User requester)
    {
        await RecordVote(guildId, target, requester, KarmaVote.VoteType.Up);
    }

    public async Task<string> GetCanonicalKeyAsync(ulong guildId, string target)
    {
        var alternateUsers = await _users.Users
            .Where(u => u.Irc!.Nick!.ToLower() == target.ToLower() || u.Discord!.Name!.ToLower() == target)
            .ToListAsync();
        if (!alternateUsers.Any())
            return target;
        var guild = _guilds.GetGuilds().FirstOrDefault(g => g.Id == guildId);
        if (guild is null)
            return target;
        var guildMembers = await _guilds.GetMembersAsync(guild);
        var match = alternateUsers.FirstOrDefault(m => guildMembers.Any(g => g.Id == m.Discord?.Id));
        return match?.Mention() ?? target;
    }

    public async Task DecrementAsync(ulong guildId, string target, User requester)
    {
        await RecordVote(guildId, target, requester, KarmaVote.VoteType.Down);
    }

    public async Task<int> GetLevelAsync(ulong guildId, string target)
    {
        target = await GetCanonicalKeyAsync(guildId, target);
        return await _karma.Karma
            .Where(k => k.GuildId == guildId)
            .Where(k => k.Key!.ToLower() == target.ToLower())
            .SumAsync(k => k.Type == KarmaVote.VoteType.Up ? 1 : -1);
    }

    public async Task<Dictionary<string, int>> GetLevelsAsync(ulong guildId) => await _karma.Karma
        .Where(k => k.GuildId == guildId)
        .GroupBy(k => k.Key!.ToLower())
        .ToDictionaryAsync(g => g.Key, g => g.Sum(k => k.Type == KarmaVote.VoteType.Up ? 1 : -1));

    public async Task RecordVote(ulong guildId, string target, User requester, KarmaVote.VoteType type)
    {
        target = await GetCanonicalKeyAsync(guildId, target);
        await AssertRateLimitAsync(guildId, target, requester);
        _karma.Karma.Add(new KarmaVote
        {
            CreatedAt = DateTime.UtcNow,
            GuildId = guildId,
            Key = target,
            VoterId = requester.Id,
            Type = type,
            Id = Guid.NewGuid()
        });
        await _karma.SaveChangesAsync();
    }

    private async Task AssertRateLimitAsync(ulong guildId, string target, User requester)
    {
        var lastUpdate = await _karma.Karma
            .Where(k => k.GuildId == guildId)
            .Where(k => k.VoterId == requester.Id)
            .Where(k => k.Key!.ToLower() == target.ToLower())
            .OrderByDescending(k => k.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastUpdate is not null)
        {
            var delay = DateTime.Now - lastUpdate.CreatedAt;
            if (delay < RateLimit)
                throw new KarmaRateLimitException(RateLimit - delay);
        }
    }
}