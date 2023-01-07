using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class ComplimentCommandModule : ApplicationCommandModule
{
    private readonly ComplimentService service;

    public ComplimentCommandModule(ComplimentService service)
    {
        this.service = service;
    }

    [SlashCommand("compliment", "Pay someone a nice (or awkward) compliment.")]
    public async Task Compliment(InteractionContext ctx, [Option("user", "hornyboi (or gal, etc)")] DiscordUser user)
    {
        var compliment = await service.GetComplimentAsync();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)}: {compliment}")
                );
    }
}
