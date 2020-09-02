using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages
{
    public class TagsModel : PageModel
    {
        private readonly BooruContext db;
        private readonly IMemoryCache cache;

        public IEnumerable<Stat> GeneralStats { get; private set; }
        public IEnumerable<TopTag> UserStats { get; private set; }

        public TagsModel(BooruContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
        }

        public async Task OnGet()
        {
            GeneralStats = await cache.GetOrCreateAsync("toptags", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await db.TagHistories.AsQueryable()
                    .GroupBy(th => th.Tag)
                    .Select(g => new Stat { Tag = g.Key, Total = g.Count() })
                    .OrderByDescending(s => s.Total)
                    .Take(20)
                    .ToListAsync();
            });
            UserStats = await cache.GetOrCreate("usertags", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return await db.GetTopTags();
            });
        }

        public struct Stat
        {
            public string Tag;
            public int Total;
        }
    }
}
