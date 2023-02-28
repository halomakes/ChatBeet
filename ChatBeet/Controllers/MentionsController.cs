using System.Threading.Tasks;
using ChatBeet.Utilities;
using DSharpPlus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MentionsController : Controller
{
    private readonly DiscordClient _discord;
    private readonly IMemoryCache _cache;

    public MentionsController(DiscordClient discord, IMemoryCache cache)
    {
        _discord = discord;
        _cache = cache;
    }

    [HttpGet("Users/{id}")]
    public async Task<ActionResult<string>> GetUserMention(ulong id) => Json(await _cache.GetOrCreateAsync($"mention:user:{id}", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(15);
        var user = await _discord.GetUserAsync(id);
        return user.DiscriminatedUsername();
    }));

    [HttpGet("Channels/{id}")]
    public async Task<ActionResult<string>> GetChannelMention(ulong id) => Json(await _cache.GetOrCreateAsync($"mention:channel:{id}", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(15);
        var channel = await _discord.GetChannelAsync(id);
        return channel.Name;
    }));

    [HttpGet("Roles/{id}")]
    public async Task<ActionResult<string>> GetRoleMention(ulong id) => Json(await _cache.GetOrCreateAsync($"mention:role:{id}", entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(15);
        var role = _discord.Guilds
            .SelectMany(g => g.Value.Roles)
            .FirstOrDefault(p => p.Key == id);
        return Task.FromResult(role.Value.Name);
    }));
}