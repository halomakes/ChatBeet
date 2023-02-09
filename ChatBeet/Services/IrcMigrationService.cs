using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Services;
public class IrcMigrationService
{
    private readonly IrcLinkContext _ctx;
    public static readonly string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public IrcMigrationService(IrcLinkContext ctx)
    {
        _ctx = ctx;
    }
    public async Task<string> GetInternalUsernameAsync(DiscordUser user)
    {
        var ircUser = await _ctx.Links.FirstOrDefaultAsync(l => l.Id == user.Id);
        return ircUser?.Nick ?? user.DiscriminatedUsername();
    }

    public async Task<string> GetInternalUsernameAsync(string username)
    {
        var (success, partialUsername, discriminator) = username.ParseUsername();
        if (!success)
            return username;
        var ircUser = await _ctx.Links.FirstOrDefaultAsync(l => l.Username.ToLower() == partialUsername.ToLower() && l.Discriminator == discriminator);
        return ircUser?.Nick ?? username;
    }

    public async Task<IEnumerable<IrcLink>> GetLinksAsync() => await _ctx.Links.ToListAsync();
}