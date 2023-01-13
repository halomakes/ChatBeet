using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class JokeCommandModule : ApplicationCommandModule
{
    private readonly DadJokeService jokeService;

    public JokeCommandModule(DadJokeService jokeService)
    {
        this.jokeService = jokeService;
    }

    [SlashCommand("joke", "Get a random (bad) joke.")]
    public async Task GetJoke(InteractionContext ctx)
    {
        var joke = await jokeService.GetDadJokeAsync();
        var responseText = string.IsNullOrEmpty(joke)
            ? "I'm the joke. 😢"
            : joke.Trim();

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(responseText));
    }
}