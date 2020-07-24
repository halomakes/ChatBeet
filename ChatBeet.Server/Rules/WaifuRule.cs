using ChatBeet;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class WaifuRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly AnilistService client;

        public WaifuRule(IOptions<ChatBeetConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(waifu|husbando) (.*)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var characterName = match.Groups[2].Value;
                var character = await client.GetCharacterAsync(characterName);

                if (character != null)
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"{character.FirstName} {character.LastName} ({character.NativeName}) - {character.LargeImageUrl} | {character.SiteUrl}",
                        Target = incomingMessage.GetResponseTarget()
                    };
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Sorry, couldn't find that {match.Groups[1].Value}.",
                        Target = incomingMessage.GetResponseTarget()
                    };
                }
            }
        }
    }
}
