using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Models;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ChatBeet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly WebIdentityService _identity;
    private readonly DiscordClient _discord;
    private readonly IMemoryCache _cache;

    public UsersController(DiscordClient discord, WebIdentityService identity, IMemoryCache cache)
    {
        _discord = discord;
        _identity = identity;
        _cache = cache;
    }

    [HttpGet("@me")]
    public async Task<ActionResult<UserViewModel>> GetCurrentUser()
    {
        var currentUser = await _identity.GetCurrentUserAsync();
        if (currentUser?.Discord?.Id is null)
            return Ok(new UserViewModel(currentUser!, Enumerable.Empty<GuildViewModel>()));
        var memberships = await Task.WhenAll(_discord.Guilds.Select(async g => (Guild: g.Value, Members: await GetMembersAsync(g.Value))));
        return Ok(new UserViewModel(
            User: currentUser,
            Guilds: memberships
                .Where(m => m.Members.Any(x => x.Id == currentUser.Discord.Id))
                .Select(m => new GuildViewModel(m.Guild))
                .ToList()
        ));
    }

    private async Task<IEnumerable<DiscordMember>> GetMembersAsync(DiscordGuild guild) => (await _cache.GetOrCreateAsync($"guild:{guild.Id}:members", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return (await guild.GetAllMembersAsync()).ToList();
    }))!;
}