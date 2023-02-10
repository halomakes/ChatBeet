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

    public async Task<IEnumerable<SuspicionReport>> GetActiveSuspicionsAsync()
    {
        var suspicions = await ActiveSuspicions
            .ToListAsync();
        return suspicions;
    }

    public async Task<IEnumerable<SuspicionReport>> GetSuspicionsAsync()
    {
        var suspicions = await _ctx.Suspicions
            .Include(s => s.Suspect)
            .ToListAsync();
        return suspicions;
    }

    private IQueryable<SuspicionReport> ActiveSuspicions => _ctx.Suspicions
        .Include(s => s.Suspect)
        .AsNoTracking()
        .AsQueryable()
        .Where(s => s.CreatedAt >= ActiveWindowStart);

    public async Task<int> GetSuspicionLevelAsync(Guid suspectId) =>
        (await GetActiveSuspicionsAsync())
        .Count(s => s.SuspectId == suspectId);

    public async Task<bool> HasRecentlyReportedAsync(Guid suspect, Guid reporter, TimeSpan debounceWindow = default)
    {
        if (debounceWindow == default)
            debounceWindow = TimeSpan.FromMinutes(2);

        var lastReport = await _ctx.Suspicions
            .AsQueryable()
            .AsNoTracking()
            .Where(s => s.ReporterId == reporter)
            .Where(s => s.SuspectId == suspect)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        return lastReport != default && (DateTime.Now - lastReport.CreatedAt) < debounceWindow;
    }

    public async Task ReportSuspiciousActivityAsync(Guid suspect, Guid reporter, bool bypassDebounceCheck = false)
    {
        if (bypassDebounceCheck || !await HasRecentlyReportedAsync(suspect, reporter))
        {
            _ctx.Suspicions.Add(new SuspicionReport
            {
                ReporterId = reporter,
                SuspectId = suspect,
                CreatedAt = DateTime.Now
            });
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SuspicionRank>> GetSuspicionLevels()
    {
        var mostSuspicious = await _ctx.Suspicions
            .Include(s => s.Suspect)
            .ThenInclude(s => s!.Preferences)
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