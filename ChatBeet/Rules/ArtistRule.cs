using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class ArtistRule : MessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly LastFmService lastFm;

        public ArtistRule(LastFmService lastFm, IOptions<IrcBotConfiguration> options)
        {
            this.lastFm = lastFm;
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
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
                        yield return new PrivateMessage(
                            incomingMessage.GetResponseTarget(),
                            $"{artist.Bio?.Summary}"
                        );
                    }

                    if (artist?.Tags?.Any() == true)
                    {
                        yield return new PrivateMessage(
                            incomingMessage.GetResponseTarget(),
                            $"{IrcValues.BOLD}Related tags{IrcValues.RESET}: { string.Join(", ", artist.Tags.Select(t => t.Name))}"
                        );
                    }

                    if (!hasBio)
                    {
                        yield return new PrivateMessage(
                            incomingMessage.GetResponseTarget(),
                            $"I found the artist but they don't have a biography.  Check here for more: {artist.Url}"
                        );
                    }
                }
                else
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        "Sorry, couldn't find that artist."
                    );
                }
            }
        }
    }
}
