using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services;
public class SuspicionService
{
    private readonly SuspicionContext _ctx;
    private readonly IrcMigrationService _irc;
    private readonly TimeSpan ActivePeriod = DateTime.Now.AddYears(2) - DateTime.Now;

    public SuspicionService(SuspicionContext ctx, IrcMigrationService irc)
    {
        _ctx = ctx;
        _irc = irc;
    }

    public DateTime ActiveWindowStart => DateTime.Now - ActivePeriod;

    public async Task<IEnumerable<Suspicion>> GetActiveSuspicionsAsync()
    {
        var links = await _irc.GetLinksAsync();
        var suspicions = await ActiveSuspicions.ToListAsync();
        foreach (var suspicion in suspicions)
        {
            suspicion.Reporter = GetInternalId(suspicion.Reporter);
            suspicion.Suspect = GetInternalId(suspicion.Suspect);
        }
        return suspicions;

        string GetInternalId(string username)
        {
            var (success, partialUsername, discriminator) = username.ParseUsername();
            if (!success)
                return username;
            var match = links.FirstOrDefault(l => l.Username.ToLower() == partialUsername.ToLower() && l.Discriminator == discriminator);
            if (match is null)
                return username;
            return match.Nick;
        }
    }

    private IQueryable<Suspicion> ActiveSuspicions => _ctx.Suspicions
        .AsNoTracking()
        .AsQueryable()
        .Where(s => s.TimeReported >= ActiveWindowStart);

    public async Task<int> GetSuspicionLevelAsync(string suspect)
    {
        var internalId = await _irc.GetInternalUsernameAsync(suspect.Trim());
        return (await GetActiveSuspicionsAsync())
            .Count(s => s.Suspect.ToLower() == internalId.ToLower());
    }

    public async Task<bool> HasRecentlyReportedAsync(string suspect, string reporter, TimeSpan debounceWindow = default)
    {
        if (debounceWindow == default)
            debounceWindow = TimeSpan.FromMinutes(2);

        var internalSuspect = await _irc.GetInternalUsernameAsync(suspect.Trim());
        var internalReporter = await _irc.GetInternalUsernameAsync(reporter.Trim());

        var lastReport = await _ctx.Suspicions
            .AsQueryable()
            .Where(s => s.Reporter.ToLower() == internalReporter.ToLower())
            .Where(s => s.Suspect.ToLower() == internalSuspect.ToLower())
            .OrderByDescending(s => s.TimeReported)
            .FirstOrDefaultAsync();

        return lastReport != default && (DateTime.Now - lastReport.TimeReported) < debounceWindow;
    }

    public async Task ReportSuspiciousActivityAsync(string suspect, string reporter, bool bypassDebounceCheck = false)
    {
        if (bypassDebounceCheck || !await HasRecentlyReportedAsync(suspect, reporter))
        {
            var internalSuspect = await _irc.GetInternalUsernameAsync(suspect.Trim());
            var internalReporter = await _irc.GetInternalUsernameAsync(reporter.Trim());

            _ctx.Suspicions.Add(new Suspicion
            {
                Reporter = internalReporter,
                Suspect = internalSuspect,
                TimeReported = DateTime.Now
            });
            await _ctx.SaveChangesAsync();
        }
    }
}
