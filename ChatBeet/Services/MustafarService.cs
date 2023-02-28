using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Exceptions;
using ChatBeet.Notifications;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Services;

public class MustafarService
{
    private static readonly Dictionary<(ulong UserId, ulong GuildId), DateTime> InvocationHistory = new();
    private readonly IHighGroundRepository _repository;
    private readonly IUsersRepository _users;
    private readonly IStatsRepository _stats;

    public const string JumpEventType = "high_ground:jump";
    public const string TripEventType = "high_ground:trip";

    public MustafarService(IHighGroundRepository repository, IUsersRepository users, IStatsRepository stats)
    {
        _repository = repository;
        _users = users;
        _stats = stats;
    }

    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);

    public async Task<HighGroundChangeNotification> ClaimAsync(ulong guildId, DiscordUser claimant)
    {
        if (InvocationHistory.TryGetValue((claimant.Id, guildId), out var lastActivation) && DateTime.Now - lastActivation < Timeout)
            throw new WimpyLegsException(lastActivation + Timeout);
        InvocationHistory[(claimant.Id, guildId)] = DateTime.Now;

        var existingStake = await _repository.Claims
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.GuildId == guildId);
        var user = await _users.GetUserAsync(claimant);
        if (existingStake is null)
        {
            _repository.Claims.Add(new()
            {
                GuildId = guildId,
                UpdatedAt = DateTime.Now,
                UserId = user.Id
            });
            await _repository.SaveChangesAsync();
            _stats.StatEvents.Add(new()
            {
                GuildId = guildId,
                TriggeringUserId = user.Id,
                EventType = JumpEventType,
                OccurredAt = DateTime.UtcNow,
            });
            await _stats.SaveChangesAsync();
            return new(null, user);
        }

        if (existingStake.UserId == user.Id)
        {
            _repository.Claims.Remove(existingStake);
            await _repository.SaveChangesAsync();
            _stats.StatEvents.Add(new()
            {
                GuildId = guildId,
                TriggeringUserId = user.Id,
                EventType = TripEventType,
                OccurredAt = DateTime.UtcNow,
            });
            await _stats.SaveChangesAsync();
            throw new StumbleException();
        }

        var oldUser = existingStake.User;
        existingStake.UserId = user.Id;
        existingStake.UpdatedAt = DateTime.Now;
        await _repository.SaveChangesAsync();
        _stats.StatEvents.Add(new()
        {
            GuildId = guildId,
            TriggeringUserId = user.Id,
            TargetedUserId = existingStake.UserId,
            EventType = JumpEventType,
            OccurredAt = DateTime.UtcNow,
        });
        await _stats.SaveChangesAsync();

        return new(oldUser, user);
    }

    public async Task<HighGround?> GetAsync(ulong guildId) => await _repository.Claims
        .Include(c => c.User)
        .FirstOrDefaultAsync(c => c.GuildId == guildId);
}