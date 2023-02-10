using ChatBeet.Data;
using ChatBeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Tags;

[Authorize]
public class UserModel : PageModel
{
    private readonly BooruContext _booru;
    private readonly IMemoryCache _cache;

    public string Nick { get; private set; }
    public IEnumerable<TagStat> Stats { get; private set; }

    public UserModel(BooruContext booru, IMemoryCache cache)
    {
        _booru = booru;
        _cache = cache;
    }

    public async Task OnGet(string nick)
    {
        Nick = nick.Trim().ToLower();
        var matchingTags = await _cache.GetOrCreateAsync($"tags:user:{nick}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            return await _booru.TagHistories
                .AsQueryable()
                .Where(th => th.Nick.ToLower() == Nick)
                .GroupBy(th => th.Tag)
                .OrderByDescending(g => g.Count())
                .Select(g => new TagStat { Tag = g.Key, Total = g.Count(), Mode = TagStat.StatMode.Tag })
                .ToListAsync();
        });
        Stats = matchingTags;
        if (matchingTags.Any())
        {
            Nick = await _booru.TagHistories.AsQueryable().Where(th => th.Nick.ToLower() == Nick).Select(th => th.Nick).FirstOrDefaultAsync();
        }
    }
}