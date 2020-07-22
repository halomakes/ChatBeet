using ChatBeet;
using DtellaRules.Services;
using DtellaRules.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class ArtistRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly LastFmService lastFm;

        public ArtistRule(LastFmService lastFm, IOptions<ChatBeetConfiguration> options)
        {
            this.lastFm = lastFm;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}artist (.*)", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Message))
            {
                var artistName = rgx.Replace(incomingMessage.Message, @"$1");

                var artist = await lastFm.GetArtistInfo(artistName);

                if (artist != null)
                {
                    // filter out empty bios
                    bool hasBio = !string.IsNullOrEmpty(artist?.Bio?.Summary) && !artist.Bio.Summary.StartsWith("<a href");

                    if (hasBio)
                    {
                        yield return new OutboundIrcMessage
                        {
                            Content = $"{artist.Bio?.Summary}",
                            Target = incomingMessage.GetResponseTarget()
                        };
                    }

                    if (artist?.Tags?.Any() == true)
                    {
                        yield return new OutboundIrcMessage
                        {
                            Content = $"{IrcValues.BOLD}Related tags{IrcValues.RESET}: { string.Join(", ", artist.Tags.Select(t => t.Name))}",
                            Target = incomingMessage.GetResponseTarget()
                        };
                    }

                    if (!hasBio)
                    {
                        yield return new OutboundIrcMessage
                        {
                            Content = $"I found the artist but they don't have a biography.  Check here for more: {artist.Url}",
                            Target = incomingMessage.GetResponseTarget()
                        };
                    }
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = "Sorry, couldn't find that artist.",
                        Target = incomingMessage.GetResponseTarget()
                    };
                }
            }
        }
    }
}
