using ChatBeet.Data;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
[SlashCommandGroup("irc", "Commands about legacy IRC integration")]
public class IrcCommandModule : ApplicationCommandModule
{
    private readonly IrcMigrationService _service;
    private readonly IrcLinkContext _db;
    private readonly DiscordClient _client;
    public const string VerifyModalId = "irc-verify";

    public IrcCommandModule(IrcMigrationService service, IrcLinkContext db, DiscordClient client)
    {
        _service = service;
        _db = db;
        _client = client;
    }

    [SlashCommand("link", "Link your old IRC nick to your Discord account.")]
    public async Task GetVerificationCode(InteractionContext ctx, [Option("irc-nick", "Your IRC nick on dtella.net")] string nick)
    {
        var existingEntry = await _db.Links.FirstOrDefaultAsync(l => l.Id == ctx.User.Id || l.Nick.ToLower() == nick.ToLower());
        if (existingEntry is not null)
        {
            var discordUser = ctx.User.Id == existingEntry.Id
                ? ctx.User
                : await _client.GetUserAsync(existingEntry.Id);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(discordUser)} is already linked to IRC user {Formatter.Bold(existingEntry.Nick)}")
                .AsEphemeral());
            return;
        }

        await _service.SendVerificationCodeAsync(ctx.Member, nick);
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
            .WithTitle("Verify Your Account")
            .WithCustomId("irc-verify")
            .AddComponents(new TextInputComponent("Token", "token", style: TextInputStyle.Paragraph, required: true))
            .AddComponents(new TextInputComponent("Nick", "nick", placeholder: nick, value: nick, required: true))
            .WithContent($"You should receive a verification code from ChatBeet in IRC.  Enter that code here to verify your account. ")
            );
    }

    private async Task LookupUser(BaseContext ctx, DiscordUser user)
    {
        var existingEntry = await _db.Links.FirstOrDefaultAsync(l => l.Id == user.Id);
        if (existingEntry is not null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} used to be {Formatter.Bold(existingEntry.Nick)} on IRC.")
                .AsEphemeral());
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} either has no IRC nick or has not linked their accounts.  Use {Formatter.InlineCode("/irc link")} to link your account.")
                .AsEphemeral());
        }
    }

    [SlashCommand("identify", "Check what a user's IRC nick was.")]
    public async Task LookupUser(InteractionContext ctx, [Option("user", "User to look up")] DiscordUser user) => await LookupUser(ctx as BaseContext, user);

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Check IRC Nick")]
    public async Task LookupUser(ContextMenuContext ctx) => await LookupUser(ctx, ctx.TargetUser);
}