using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class GoogleCommandModule : ApplicationCommandModule
{
    private readonly GoogleSearchService searchService;

    public GoogleCommandModule(GoogleSearchService searchService)
    {
        this.searchService = searchService;
    }

    [SlashCommand("google", "Find something on the interwebz")]
    public async Task Compliment(InteractionContext ctx, [Option("query", "Thing to search for")] string query)
    {
        var resultLink = await searchService.GetFeelingLuckyResultAsync(query);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(resultLink.ToString())
        );
    }
}
