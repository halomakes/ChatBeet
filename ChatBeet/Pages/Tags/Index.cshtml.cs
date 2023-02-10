using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Tags;

public class IndexModel : PageModel
{
    private readonly BooruContext _db;
    private readonly IMemoryCache _cache;
    private static readonly Random rng = new();

    public IEnumerable<TagStat> GeneralStats { get; private set; }
    public IEnumerable<TagStat> RandomStats { get; private set; }
    public IEnumerable<TopTag> UserStats { get; private set; }
    public DateTime Earliest { get; private set; }

    public IndexModel(BooruContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task OnGet()
    {
        GeneralStats = (await GetStats())
            .OrderByDescending(s => s.Total)
            .Take(20)
            .ToList();
        RandomStats = (await GetStats())
            .OrderBy(s => rng.Next())
            .Take(20)
            .ToList();
        UserStats = await _cache.GetOrCreate("tags:user", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return await _db.GetTopTags();
        });
        Earliest = await _cache.GetOrCreate("tags:date", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return await EntityFrameworkQueryableExtensions.MinAsync(_db.TagHistories, th => th.Timestamp);
        });
    }

    private async Task<IEnumerable<TagStat>> GetStats() => await _cache.GetOrCreateAsync("tags:stats", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
        return await _db.TagHistories.AsQueryable()
            .GroupBy(th => th.Tag)
            .Select(g => new TagStat { Tag = g.Key, Total = g.Count(), Mode = TagStat.StatMode.Tag })
            .ToListAsync();
    });
}