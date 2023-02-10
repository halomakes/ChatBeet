using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class KeywordService
{
    private readonly KeywordContext _db;
    private readonly IMemoryCache _cache;

    public static DateTime StatsLastUpdated { get; private set; }

    public KeywordService(KeywordContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public Task<List<Keyword>> GetKeywordsAsync() => _cache.GetOrCreateAsync("keywords", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return await _db.Keywords.AsQueryable().ToListAsync();
    });

    public async Task<Keyword> GetKeywordAsync(int id)
    {
        var keywords = await GetKeywordsAsync();
        return keywords.FirstOrDefault(k => k.Id == id);
    }

    public Task<KeywordStat> GetKeywordStatAsync(int id) => _cache.GetOrCreateAsync($"keywords:stats:{id}", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        var keyword = await GetKeywordAsync(id);
        var stats = await _db.Records
            .AsQueryable()
            .Where(r => r.KeywordId == keyword.Id)
            .GroupBy(r => r.Nick)
            .OrderByDescending(g => g.Count())
            .Select(g => new KeywordStat.UserKeywordStat
            {
                Nick = g.Key,
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
        var allStats = await _db.Records
            .AsQueryable()
            .GroupBy(r => new { r.KeywordId, r.Nick })
            .OrderByDescending(g => g.Count())
            .Select(g => new
            {
                g.Key.KeywordId,
                Stats = new KeywordStat.UserKeywordStat
                {
                    Nick = g.Key.Nick,
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