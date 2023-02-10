using System.Security.Claims;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using DSharpPlus;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Services;

public class WebIdentityService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly DiscordClient _discordClient;
    private readonly IUsersRepository _usersRepository;

    public WebIdentityService(IHttpContextAccessor contextAccessor, DiscordClient discordClient, IUsersRepository usersRepository)
    {
        _contextAccessor = contextAccessor;
        _discordClient = discordClient;
        _usersRepository = usersRepository;
    }

    public async Task<User> GetCurrentUserAsync()
    {
        var user = _contextAccessor.HttpContext?.User;
        if (!user?.Identity?.IsAuthenticated ?? false)
            return null;

        var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var discordId = ulong.Parse(id);
        var existingUser = await _usersRepository.Users.FirstOrDefaultAsync(u => u.Discord!.Id == discordId);
        return existingUser ?? await CreateCurrentUserAsync(user);
    }

    private async Task<User> CreateCurrentUserAsync(ClaimsPrincipal user)
    {
        var id = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var username = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var discriminator = user.Claims.FirstOrDefault(c => c.Type == "urn:discord:user:discriminator")?.Value;
        var createdUser = _usersRepository.Users.Add(new()
        {
            Id = Guid.NewGuid(),
            Discord = new()
            {
                Id = ulong.Parse(id!),
                Name = username,
                Discriminator = discriminator
            }
        });
        await _usersRepository.SaveChangesAsync();
        return createdUser.Entity;
    }
}