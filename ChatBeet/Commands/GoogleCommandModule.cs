using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class GoogleCommandModule : ApplicationCommandModule
{
    private readonly GoogleSearchService _searchService;

    public GoogleCommandModule(GoogleSearchService searchService)
    {
        _searchService = searchService;
    }

    [SlashCommand("google", "Find something on the interwebz")]
    public async Task Compliment(InteractionContext ctx, [Option("query", "Thing to search for")] string query)
    {
        var resultLink = await _searchService.GetFeelingLuckyResultAsync(query);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(resultLink.ToString())
        );
    }
}
