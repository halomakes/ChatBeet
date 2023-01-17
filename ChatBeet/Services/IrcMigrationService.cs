using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using GravyBot;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ChatBeet.Services.LogonService;

namespace ChatBeet.Services;
public class IrcMigrationService
{
    private readonly MessageQueueService _messageQueue;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IrcLinkContext _ctx;
    private static readonly string TokenAction = "passwordless-auth";
    private static readonly string TokenProvider = "Default";
    public static readonly string Scheme = CookieAuthenticationDefaults.AuthenticationScheme;

    public IrcMigrationService(MessageQueueService messageQueue, UserManager<IdentityUser> userManager, IrcLinkContext ctx)
    {
        _messageQueue = messageQueue;
        _userManager = userManager;
        _ctx = ctx;
    }

    public async Task SendVerificationCodeAsync(DiscordMember user, string nick)
    {
        var ircUser = await GetUserAsync(nick);
        if (ircUser == default)
        {
            ircUser = new IdentityUser(nick) { UserName = nick };
            await _userManager.CreateAsync(ircUser);
        }

        var token = await _userManager.GenerateUserTokenAsync(ircUser, TokenProvider, TokenAction);

        _messageQueue.Push(new IrcLinkRequest { Nick = nick, AuthToken = token, User = user });
    }

    public Task<IdentityUser> GetUserAsync(string nick) => _userManager.FindByNameAsync(nick.Trim().ToLower());

    public async Task ValidateTokenAsync(DiscordInteraction interaction, string nick, string token)
    {
        var member = interaction.User as DiscordMember;
        var user = await GetUserAsync(nick);
        if (user == default)
            throw new UserNotFoundException();

        var isValid = await _userManager.VerifyUserTokenAsync(user, TokenProvider, TokenAction, token);

        if (!isValid)
        {
            await interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"The token you entered was invalid."));
        }
        else
        {
            _ctx.Links.Add(new()
            {
                Id = member.Id,
                Nick = nick,
                Discriminator = member.Discriminator,
                Username = member.Username
            });
            await _ctx.SaveChangesAsync();
            await _userManager.UpdateSecurityStampAsync(user);
            await interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Your account has been linked to IRC user {nick}."));

            return;
        }
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