using ChatBeet.Data;
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
    public class TagModel : PageModel
    {
        private readonly BooruContext booru;
        private readonly IMemoryCache cache;

        public string TagName { get; private set; }
        public IEnumerable<TagStat> Stats { get; private set; }

        public TagModel(BooruContext booru, IMemoryCache cache)
        {
            this.booru = booru;
            this.cache = cache;
        }

        public async Task OnGet(string tagName)
        {
            TagName = tagName.Trim().ToLower();
            var matchingTags = await cache.GetOrCreateAsync($"tags:tag:{tagName}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                return await booru.TagHistories
                    .AsQueryable()
                    .Where(th => th.Tag.ToLower() == tagName)
                    .GroupBy(th => th.Nick)
                    .OrderByDescending(g => g.Count())
                    .Select(g => new TagStat { Tag = g.Key, Total = g.Count() })
                    .ToListAsync();
            });
            Stats = matchingTags;
        }
    }
}