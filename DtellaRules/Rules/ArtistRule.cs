using ChatBeet;
using DtellaRules.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class ArtistRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly LastFmService lastFm;

        public ArtistRule(LastFmService lastFm, IOptions<ChatBeetConfiguration> options)
        {
            this.lastFm = lastFm;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}artist (.*)");
            if (rgx.IsMatch(incomingMessage.Content))
            {
                var artistName = rgx.Replace(incomingMessage.Content, @"$1");

                var artist = await lastFm.GetArtistInfo(artistName);

                if (artist != null)
                {
                    // filter out empty bios
                    bool hasBio = !string.IsNullOrEmpty(artist?.Bio?.Summary) && !artist.Bio.Summary.StartsWith("<a href");

                    if (hasBio)
                        yield return new OutboundIrcMessage
                        {
                            Content = $"{artist.Bio?.Summary}",
                            Target = incomingMessage.Channel
                        };

                    if (artist?.Tags?.Any() == true)
                        yield return new OutboundIrcMessage
                        {
                            Content = $"Related tags: { string.Join(", ", artist.Tags.Select(t => t.Name))}",
                            Target = incomingMessage.Channel
                        };

                    if (!hasBio)
                        yield return new OutboundIrcMessage
                        {
                            Content = $"I found the artist but they don't have a biography.  Check here for more: {artist.Url}",
                            Target = incomingMessage.Channel
                        };
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = "Sorry, couldn't find that artist.",
                        Target = incomingMessage.Channel
                    };
                }
            }
        }
    }

    public class TrackRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly LastFmService lastFm;

        public TrackRule(LastFmService lastFm, IOptions<ChatBeetConfiguration> options)
        {
            this.lastFm = lastFm;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}track (.*) by (.*)");
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var trackName = match.Groups[1].Value;
                var artistName = match.Groups[2].Value;

                var track = await lastFm.GetTrackInfo(trackName, artistName);

                if (track != null)
                    yield return new OutboundIrcMessage
                    {
                        Content = $"{track.Name} ({track.Duration}) - from {track.AlbumName} by {track.ArtistName} - {track.Url}",
                        Target = incomingMessage.Channel
                    };
                else
                    yield return new OutboundIrcMessage
                    {
                        Content = "Sorry, couldn't find that track.",
                        Target = incomingMessage.Channel
                    };
            }
        }
    }
}
