using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class ComplimentCommandModule : ApplicationCommandModule
{
    private readonly ComplimentService _service;

    public ComplimentCommandModule(ComplimentService service)
    {
        _service = service;
    }

    [SlashCommand("compliment", "Pay someone a nice (or awkward) compliment")]
    public async Task Compliment(InteractionContext ctx, [Option("user", "hornyboi (or gal, etc)")] DiscordUser user)
    {
        var compliment = await _service.GetComplimentAsync();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)}: {compliment}")
                );
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Compliment")]
    public async Task Compliment(ContextMenuContext ctx)
    {
        var compliment = await _service.GetComplimentAsync();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(ctx.TargetUser)}: {compliment}")
                );
    }
}
