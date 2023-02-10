using System.Threading.Tasks;
using ChatBeet.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
[SlashCommandGroup("irc", "Commands about legacy IRC integration")]
public class IrcCommandModule : ApplicationCommandModule
{
    private readonly IUsersRepository _db;

    public IrcCommandModule(IUsersRepository db)
    {
        _db = db;
    }

    private async Task LookupUser(BaseContext ctx, DiscordUser user)
    {
        var existingEntry = await _db.Users.FirstOrDefaultAsync(l => l.Discord!.Id == user.Id);
        if (existingEntry?.Irc?.Nick is not null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} used to be {Formatter.Bold(existingEntry.Irc!.Nick)} on IRC.")
                .AsEphemeral());
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} either has no IRC nick or has not linked their accounts. ")
                .AsEphemeral());
        }
    }

    [SlashCommand("identify", "Check what a user's IRC nick was")]
    public async Task LookupUser(InteractionContext ctx, [Option("user", "User to look up")] DiscordUser user) => await LookupUser(ctx as BaseContext, user);

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Check IRC Nick")]
    public async Task LookupUser(ContextMenuContext ctx) => await LookupUser(ctx, ctx.TargetUser);
}