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

namespace ChatBeet.Pages.Tags
{
    public class IndexModel : PageModel
    {
        private readonly BooruContext db;
        private readonly IMemoryCache cache;
        private static readonly Random rng = new Random();

        public IEnumerable<TagStat> GeneralStats { get; private set; }
        public IEnumerable<TagStat> RandomStats { get; private set; }
        public IEnumerable<TopTag> UserStats { get; private set; }
        public DateTime Earliest { get; private set; }

        public IndexModel(BooruContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
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
            UserStats = await cache.GetOrCreate("tags:user", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await db.GetTopTags();
            });
            Earliest = await cache.GetOrCreate("tags:date", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await EntityFrameworkQueryableExtensions.MinAsync(db.TagHistories, th => th.Timestamp);
            });
        }

        private async Task<IEnumerable<TagStat>> GetStats() => await cache.GetOrCreateAsync("tags:stats", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            return await db.TagHistories.AsQueryable()
                .GroupBy(th => th.Tag)
                .Select(g => new TagStat { Tag = g.Key, Total = g.Count() })
                .ToListAsync();
        });
    }
}