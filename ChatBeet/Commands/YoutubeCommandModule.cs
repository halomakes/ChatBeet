using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using YoutubeExplode;

namespace ChatBeet.Commands;
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class YoutubeCommandModule : ApplicationCommandModule
{
    private readonly YoutubeClient _client;

    public YoutubeCommandModule(YoutubeClient client)
    {
        _client = client;
    }

    [SlashCommand("youtube", "Search for content on YouTube")]
    public async Task SearchContent(InteractionContext ctx, [Option("query", "What to search for")] string query, [Option("type", "Type of content to search for")] QueryType type = QueryType.Unspecified)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var firstMatch = type switch
        {
            QueryType.Video => await _client.Search.GetVideosAsync(query).FirstOrDefaultAsync(),
            QueryType.Playlist => await _client.Search.GetPlaylistsAsync(query).FirstOrDefaultAsync(),
            QueryType.Channel => await _client.Search.GetChannelsAsync(query).FirstOrDefaultAsync(),
            _ => await _client.Search.GetResultsAsync(query).FirstOrDefaultAsync()
        };
        if (firstMatch is null)
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
            .WithContent($"Sorry, couldn't find anything for {query}"));
        }
        else
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent(firstMatch.Url));
        }
    }

    public enum QueryType
    {
        Unspecified,
        Video,
        Playlist,
        Channel
    }
}
