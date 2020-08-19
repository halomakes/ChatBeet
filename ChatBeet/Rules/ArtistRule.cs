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
    public class ArtistRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly LastFmService lastFm;
        private readonly Regex pattern;

        public ArtistRule(LastFmService lastFm, IOptions<IrcBotConfiguration> options)
        {
            this.lastFm = lastFm;
            config = options.Value;
            pattern = new Regex($"^{Regex.Escape(config.CommandPrefix)}artist (.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => pattern.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            if (pattern.IsMatch(incomingMessage.Message))
            {
                var artistName = pattern.Replace(incomingMessage.Message, @"$1");

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
