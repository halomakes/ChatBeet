using ChatBeet;
using DtellaRules.Utilities;
using Microsoft.Extensions.Options;
using Miki.Anilist;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class WaifuRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly AnilistClient client;

        public WaifuRule(IOptions<ChatBeetConfiguration> options, AnilistClient client)
        {
            config = options.Value;
            this.client = client;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(waifu|husbando) (.*)");
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var characterName = match.Groups[2].Value;
                var character = await client.GetCharacterAsync(characterName);

                if (character != null)
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"{character.FirstName} {character.LastName} ({character.NativeName}) - {character.LargeImageUrl}",
                        Target = incomingMessage.Channel
                    };
                    var description = character.GetSimplifiedDescription();
                    if (!string.IsNullOrEmpty(description))
                    {
                        if (description.ExceedsMaxLength())
                        {
                            yield return new OutboundIrcMessage
                            {
                                Content = description.TruncateMessage(),
                                Target = incomingMessage.Channel
                            };
                            yield return new OutboundIrcMessage
                            {
                                Content = $"See more at {character.SiteUrl}",
                                Target = incomingMessage.Channel
                            };
                        }
                        else
                        {
                            yield return new OutboundIrcMessage
                            {
                                Content = description,
                                Target = incomingMessage.Channel
                            };
                        }
                    }
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Sorry, couldn't find that {match.Groups[1].Value}.",
                        Target = incomingMessage.Channel
                    };
                }
            }
        }
    }
}
