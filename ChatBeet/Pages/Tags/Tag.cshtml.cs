using ChatBeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Pages.Tags;

[Authorize]
public class TagModel : PageModel
{
    private readonly IBooruRepository _booru;
    private readonly IMemoryCache _cache;

    public string TagName { get; private set; }
    public IEnumerable<TagStat> Stats { get; private set; }

    public TagModel(IBooruRepository booru, IMemoryCache cache)
    {
        _booru = booru;
        _cache = cache;
    }

    public async Task OnGet(string tagName)
    {
        TagName = tagName.Trim().ToLower();
        var matchingTags = await _cache.GetOrCreateAsync($"tags:tag:{tagName}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            var entries = await _booru.TagHistories
                .Include(h => h.User)
                .AsQueryable()
                .Where(th => th.Tag.ToLower() == tagName)
                .GroupBy(th => th.User)
                .OrderByDescending(g => g.Count())
                .ToListAsync();
            return entries.Select(g => new TagStat { Tag = g.Key.DisplayName(), Total = g.Count(), Mode = TagStat.StatMode.User });
        });
        Stats = matchingTags;
    }
}