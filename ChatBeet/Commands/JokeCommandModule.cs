using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class JokeCommandModule : ApplicationCommandModule
{
    private readonly DadJokeService _jokeService;

    public JokeCommandModule(DadJokeService jokeService)
    {
        _jokeService = jokeService;
    }

    [SlashCommand("joke", "Get a random (bad) joke")]
    public async Task GetJoke(InteractionContext ctx)
    {
        var joke = await _jokeService.GetDadJokeAsync();
        var responseText = string.IsNullOrEmpty(joke)
            ? "I'm the joke. 😢"
            : joke.Trim();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(responseText));
    }
}