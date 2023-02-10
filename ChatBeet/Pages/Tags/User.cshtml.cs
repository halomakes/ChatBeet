using ChatBeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Pages.Tags;

[Authorize]
public class UserModel : PageModel
{
    private readonly IBooruRepository _booru;
    private readonly IMemoryCache _cache;
    private readonly IUsersRepository _users;

    public User User { get; private set; }
    public IEnumerable<TagStat> Stats { get; private set; }

    public UserModel(IBooruRepository booru, IMemoryCache cache, IUsersRepository users)
    {
        _booru = booru;
        _cache = cache;
        _users = users;
    }

    public async Task OnGet(Guid userId)
    {
        var matchingTags = await _cache.GetOrCreateAsync($"tags:user:{userId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            return await _booru.TagHistories
                .AsQueryable()
                .Where(th => th.UserId == userId)
                .GroupBy(th => th.Tag)
                .OrderByDescending(g => g.Count())
                .Select(g => new TagStat { Tag = g.Key, Total = g.Count(), Mode = TagStat.StatMode.Tag })
                .ToListAsync();
        });
        Stats = matchingTags;
        User = await _users.Users.FindAsync(userId);
    }
}