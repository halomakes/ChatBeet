using ChatBeet;
using DtellaRules.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class WaifuRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly AnilistService client;

        public WaifuRule(IOptions<ChatBeetConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(waifu|husbando) (.*)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var characterName = match.Groups[2].Value;
                var character = await client.GetCharacterAsync(characterName);

                if (character != null)
                    yield return new OutboundIrcMessage
                    {
                        Content = $"{character.FirstName} {character.LastName} ({character.NativeName}) - {character.LargeImageUrl} | {character.SiteUrl}",
                        Target = incomingMessage.Channel
                    };
                else
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Sorry, couldn't find that {match.Groups[1].Value}.",
                        Target = incomingMessage.Channel
                    };
            }
        }
    }
}
