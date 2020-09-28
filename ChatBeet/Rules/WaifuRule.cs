using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class WaifuRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly AnilistService client;
        private readonly Regex rgx;

        public WaifuRule(IOptions<IrcBotConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
            rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}(waifu|husbando) (.*)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var characterName = match.Groups[2].Value;
                var character = await client.GetCharacterAsync(characterName);

                if (character != null)
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"{character.FirstName} {character.LastName} ({character.NativeName}) - {character.LargeImageUrl} | {character.SiteUrl}"
                    );
                }
                else
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"Sorry, couldn't find that {match.Groups[1].Value}."
                    );
                }
            }
        }
    }
}
