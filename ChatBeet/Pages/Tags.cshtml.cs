using ChatBeet.Data;
using ChatBeet.Utilities;
using LinqToTwitter;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IEnumerable<Stat> GeneralStats;

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
                  var topTags = await db.TagHistories.AsQueryable()
                      .GroupBy(th => th.Tag)
                      .Select(g => new Stat { Tag = g.Key, Total = g.Count() })
                      .OrderByDescending(s => s.Total)
                      .Take(20)
                      .ToListAsync();
                  return topTags.Shuffle();
              });
        }

        public struct Stat
        {
            public string Tag;
            public int Total;
        }
    }
}
