using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Services;
using DSharpPlus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly DiscordClient _discord;
    private readonly WebIdentityService _identity;
    private readonly GuildService _guilds;
    private readonly IUsersRepository _users;

    public UsersController(WebIdentityService identity, GuildService guilds, DiscordClient discord, IUsersRepository users)
    {
        _identity = identity;
        _guilds = guilds;
        _discord = discord;
        _users = users;
    }

    [HttpGet("@me")]
    [Authorize]
    public async Task<ActionResult<UserViewModel>> GetCurrentUser()
    {
        var currentUser = await _identity.GetCurrentUserAsync();
        if (currentUser.Discord?.Id is null)
            return Ok(new UserViewModel(currentUser, Enumerable.Empty<GuildViewModel>(), null));
        var memberships = await Task.WhenAll(_guilds.GetGuilds().Select(async g => (Guild: g, Members: await _guilds.GetMembersAsync(g))));
        var discordUser = await _discord.GetUserAsync(currentUser.Discord.Id.Value);
        return Ok(new UserViewModel(
            User: currentUser,
            Guilds: memberships
                .Where(m => m.Members.Any(x => x.Id == currentUser.Discord.Id))
                .Select(m => new GuildViewModel(m.Guild))
                .ToList(),
            AvatarUrl: discordUser.AvatarUrl
        ));
    }

    [HttpGet("{userId}/Color")]
    public async Task<ActionResult<string?>> GetUserColor([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var color = await _users.UserPreferences
            .Where(p => p.Preference == UserPreference.GearColor)
            .Where(p => p.UserId == userId)
            .Select(p => p.Value)
            .FirstOrDefaultAsync(cancellationToken);
        return Json(color);
    }
}