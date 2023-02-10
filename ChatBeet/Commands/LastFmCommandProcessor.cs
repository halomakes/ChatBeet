using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;

namespace ChatBeet.Commands;

[SlashCommandGroup("music", "Commands related to music")]
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class LastFmCommandModule : ApplicationCommandModule
{
    private readonly LastFmService _lastFm;

    public LastFmCommandModule(LastFmService lastFm)
    {
        _lastFm = lastFm;
    }

    [SlashCommand("artist", "Look up an artist on Last.fm")]
    public async Task GetArtist(InteractionContext ctx, [Option("name", "Name of musician to look up")] string name)
    {
        var artist = await _lastFm.GetArtistInfo(name);

        if (artist != null)
        {
            // filter out empty bios
            bool hasBio = !string.IsNullOrEmpty(artist?.Bio?.Summary) && !artist.Bio.Summary.StartsWith("<a href");
            var content = new StringBuilder();

            if (artist?.Tags?.Any() == true)
                content.AppendLine($"{Formatter.Bold("Related tags")}: {string.Join(", ", artist.Tags.Select(t => t.Name))}");

            var embed = new DiscordEmbedBuilder()
                .WithTitle(artist.Name)
                .WithUrl(artist.Url)
                .WithDescription(artist.Bio?.Summary?.Truncate(250));

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(content.ToString())
                .AddEmbed(embed)
                );
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find that artist.")
                );
        }
    }

    [SlashCommand("song", "Look up a track on Last.fm")]
    public async Task GetTrack(InteractionContext ctx, [Option("track", "Name of the track to search for")] string trackName, [Option("artist", "Name of artist who recorded the track")] string artistName)
    {
        var track = await _lastFm.GetTrackInfo(trackName, artistName);

        if (track != null)
        {
            var result = new StringBuilder(Formatter.Bold(track.Name));
            if (track.Duration.HasValue)
                result.Append($" ({track.Duration})");

            if (!string.IsNullOrEmpty(track.AlbumName))
                result.Append($" from {Formatter.Bold(track.AlbumName)}");

            if (!string.IsNullOrEmpty(track.ArtistName))
                result.Append($" by {Formatter.Bold(track.ArtistName)}");

            result.Append($" | {track.Url}");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(result.ToString())
                );
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find that song.")
                );
        }
    }
}
