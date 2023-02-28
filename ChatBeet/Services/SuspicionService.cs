using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Models;

namespace ChatBeet.Services;

public class SuspicionService
{
    private readonly ISuspicionRepository _ctx;
    private readonly TimeSpan _activePeriod = DateTime.Now.AddYears(2) - DateTime.Now;

    public SuspicionService(ISuspicionRepository ctx)
    {
        _ctx = ctx;
    }

    public DateTime ActiveWindowStart => DateTime.Now - _activePeriod;

    public async Task<IEnumerable<SuspicionReport>> GetActiveSuspicionsAsync(ulong guildId)
    {
        var suspicions = await ActiveSuspicions
            .Where(s => s.GuildId == guildId)
            .ToListAsync();
        return suspicions;
    }

    public async Task<IEnumerable<SuspicionReport>> GetSuspicionsAsync(ulong guildId)
    {
        var suspicions = await _ctx.Suspicions
            .Include(s => s.Suspect)
            .Where(s => s.GuildId == guildId)
            .ToListAsync();
        return suspicions;
    }

    private IQueryable<SuspicionReport> ActiveSuspicions => _ctx.Suspicions
        .Include(s => s.Suspect)
        .AsNoTracking()
        .AsQueryable()
        .Where(s => s.CreatedAt >= ActiveWindowStart);

    public async Task<int> GetSuspicionLevelAsync(ulong guildId, Guid suspectId) =>
        (await GetActiveSuspicionsAsync(guildId))
        .Count(s => s.SuspectId == suspectId);

    public async Task<bool> HasRecentlyReportedAsync(ulong guildId, Guid suspect, Guid reporter, TimeSpan debounceWindow = default)
    {
        if (debounceWindow == default)
            debounceWindow = TimeSpan.FromMinutes(2);

        var lastReport = await _ctx.Suspicions
            .AsNoTracking()
            .Where(s => s.GuildId == guildId)
            .Where(s => s.ReporterId == reporter)
            .Where(s => s.SuspectId == suspect)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        return lastReport != default && DateTime.Now - lastReport.CreatedAt < debounceWindow;
    }

    public async Task ReportSuspiciousActivityAsync(ulong guildId, Guid suspect, Guid reporter, bool bypassDebounceCheck = false)
    {
        if (bypassDebounceCheck || !await HasRecentlyReportedAsync(guildId, suspect, reporter))
        {
            _ctx.Suspicions.Add(new SuspicionReport
            {
                GuildId = guildId,
                ReporterId = reporter,
                SuspectId = suspect,
                CreatedAt = DateTime.UtcNow
            });
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SuspicionRank>> GetSuspicionLevels(ulong guildId)
    {
        var mostSuspicious = await _ctx.Suspicions
            .Include(s => s.Suspect)
            .ThenInclude(s => s!.Preferences)
            .Where(s => s.GuildId == guildId)
            .GroupBy(s => s.Suspect!.Id)
            .Select(g => new
            {
                User = g.First().Suspect,
                Active = g.Count(grp => grp.CreatedAt >= ActiveWindowStart),
                Total = g.Count()
            })
            .OrderByDescending(t => t.Active)
            .ToListAsync();

        return mostSuspicious.Select(s =>
        {
            var pref = s.User!.Preferences!.FirstOrDefault(p => p.Preference == UserPreference.GearColor);
            return new SuspicionRank
            {
                User = s.User,
                Level = s.Active,
                LifetimeLevel = s.Total,
                Color = pref?.Value
            };
        }).ToList();
    }
}