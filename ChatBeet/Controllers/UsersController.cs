using System.Threading.Tasks;
using ChatBeet.Models;
using ChatBeet.Services;
using DSharpPlus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : Controller
{
    private readonly DiscordClient _discord;
    private readonly WebIdentityService _identity;
    private readonly GuildService _guilds;

    public UsersController(WebIdentityService identity, GuildService guilds, DiscordClient discord)
    {
        _identity = identity;
        _guilds = guilds;
        _discord = discord;
    }

    [HttpGet("@me")]
    public async Task<ActionResult<UserViewModel>> GetCurrentUser()
    {
        var currentUser = await _identity.GetCurrentUserAsync();
        if (currentUser?.Discord?.Id is null)
            return Ok(new UserViewModel(currentUser!, Enumerable.Empty<GuildViewModel>(), null));
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
}