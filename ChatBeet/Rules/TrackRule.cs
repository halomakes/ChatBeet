using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class TrackRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly LastFmService lastFm;
        private readonly Regex rgx;

        public TrackRule(LastFmService lastFm, IOptions<IrcBotConfiguration> options)
        {
            this.lastFm = lastFm;
            config = options.Value;
            rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}track (.*) by (.*)");
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var trackName = match.Groups[1].Value;
                var artistName = match.Groups[2].Value;

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
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), result);
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), "Sorry, couldn't find that track.");
                }
            }
        }
    }
}
