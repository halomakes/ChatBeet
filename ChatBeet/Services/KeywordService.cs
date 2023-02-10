using ChatBeet.Data.Entities;
using ChatBeet.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Services;

public class KeywordService
{
    private readonly IKeywordsRepository _db;
    private readonly IMemoryCache _cache;

    public static DateTime StatsLastUpdated { get; private set; }

    public KeywordService(IKeywordsRepository db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public Task<List<Keyword>> GetKeywordsAsync() => _cache.GetOrCreateAsync("keywords", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return await _db.Keywords.AsQueryable().ToListAsync();
    });

    public async Task<Keyword> GetKeywordAsync(Guid id)
    {
        var keywords = await GetKeywordsAsync();
        return keywords.FirstOrDefault(k => k.Id == id);
    }

    public Task<KeywordStat> GetKeywordStatAsync(Guid id) => _cache.GetOrCreateAsync($"keywords:stats:{id}", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var keyword = await GetKeywordAsync(id);
        var stats = await _db.Hits
            .Include(h => h.User)
            .AsQueryable()
            .Where(r => r.KeywordId == keyword.Id)
            .GroupBy(r => r.UserId)
            .OrderByDescending(g => g.Count())
            .Select(g => new KeywordStat.UserKeywordStat
            {
                User = g.First().User,
                Hits = g.Count(),
                Excerpt = g.Min(gp => gp.Message)
            })
            .ToListAsync();

        return new KeywordStat
        {
            Keyword = keyword,
            Stats = stats
        };
    });

    public Task<IEnumerable<KeywordStat>> GetKeywordStatsAsync() => _cache.GetOrCreateAsync("keyword:stats", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        StatsLastUpdated = DateTime.UtcNow;

        var keywords = await GetKeywordsAsync();
        var allStats = await _db.Hits
            .Include(h => h.User)
            .AsQueryable()
            .GroupBy(r => new { r.KeywordId, r.UserId })
            .OrderByDescending(g => g.Count())
            .Select(g => new
            {
                g.Key.KeywordId,
                Stats = new KeywordStat.UserKeywordStat
                {
                    User = g.First().User,
                    Hits = g.Count(),
                    Excerpt = g.Min(gp => gp.Message)
                }
            })
            .ToListAsync();
        return keywords.Select(keyword => new KeywordStat
        {
            Keyword = keyword,
            Stats = allStats
                .Where(s => s.KeywordId == keyword.Id)
                .Select(s => s.Stats)
        });
    });
}