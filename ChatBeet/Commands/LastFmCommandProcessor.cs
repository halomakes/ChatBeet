using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class LastFmCommandProcessor : CommandProcessor
    {
        private readonly LastFmService lastFm;

        public LastFmCommandProcessor(LastFmService lastFm)
        {
            this.lastFm = lastFm;
        }

        [Command("artist {name}")]
        public async IAsyncEnumerable<IClientMessage> GetArtist(string name)
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
                        $"{IrcValues.BOLD}Related tags{IrcValues.RESET}: { string.Join(", ", artist.Tags.Select(t => t.Name))}"
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

        [Command("track {trackName} by {artistName}")]
        public async Task<IClientMessage> GetTrack(string trackName, string artistName)
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
        }
    }
}
