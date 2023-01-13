using ChatBeet.Services;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class LastFmCommandProcessor : ApplicationCommandModule
{
    private readonly LastFmService lastFm;

    public LastFmCommandProcessor(LastFmService lastFm)
    {
        this.lastFm = lastFm;
    }

    /*[SlashCommand("artist", "Look up an artist on Last.fm")]
    public async IAsyncEnumerable<IClientMessage> GetArtist([Option("name", "Name of musician to look up")] string name)
    {
        var artist = await lastFm.GetArtistInfo(name);

        if (artist != null)
        {
            // filter out empty bios
            bool hasBio = !string.IsNullOrEmpty(artist?.Bio?.Summary) && !artist.Bio.Summary.StartsWith("<a href");

            if (hasBio)
            {
                yield return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"{artist.Bio?.Summary}"
                );
            }

            if (artist?.Tags?.Any() == true)
            {
                yield return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"{IrcValues.BOLD}Related tags{IrcValues.RESET}: {string.Join(", ", artist.Tags.Select(t => t.Name))}"
                );
            }

            if (!hasBio)
            {
                yield return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"I found the artist but they don't have a biography.  Check here for more: {artist.Url}"
                );
            }
        }
        else
        {
            yield return new PrivateMessage(
                IncomingMessage.GetResponseTarget(),
                "Sorry, couldn't find that artist."
            );
        }
    }

    [SlashCommand("track {trackName} by {artistName}", "Look up a track on Last.fm")]
    public async Task<IClientMessage> GetTrack([Required] string trackName, [Required] string artistName)
    {
        var track = await lastFm.GetTrackInfo(trackName, artistName);

        if (track != null)
        {
            var result = $"{IrcValues.BOLD}{track.Name}{IrcValues.RESET}";
            if (track.Duration.HasValue)
            {
                result += $" ({track.Duration})";
            }

            if (!string.IsNullOrEmpty(track.AlbumName))
            {
                result += $" from {IrcValues.BOLD}{track.AlbumName}{IrcValues.RESET}";
            }

            if (!string.IsNullOrEmpty(track.ArtistName))
            {
                result += $" by {IrcValues.BOLD}{track.ArtistName}{IrcValues.BOLD}";
            }

            result += $" | {track.Url}";
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), result);
        }
        else
        {
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find that track.");
        }
    }*/
}
