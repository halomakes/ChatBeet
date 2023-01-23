using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class MiataCommandModule : ApplicationCommandModule
{
    private readonly MemeService _memes;

    public MiataCommandModule(MemeService memes)
    {
        _memes = memes;
    }

    [SlashCommand("miata", "Get a random Miata meme")]
    public async Task GetMiataMeme(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var embed = await _memes.GetRandomImageAsync("miata");
        await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder().WithImageUrl(embed)));
    }
}