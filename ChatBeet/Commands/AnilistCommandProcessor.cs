using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Miki.Anilist;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class AnilistCommandProcessor : CommandProcessor
    {
        private readonly AnilistService client;

        public AnilistCommandProcessor(AnilistService client)
        {
            this.client = client;
        }

        [Command("anime {query}")]
        [Command("manga {query}")]
        [Command("ln {query}")]
        [Command("ova {query}")]
        public async Task<IClientMessage> GetMedia(string query)
        {
            var type = TriggeringCommandName switch
            {
                "anime" => MediaType.ANIME,
                "manga" => MediaType.MANGA,
                "ln" => MediaType.MANGA,
                "light novel" => MediaType.MANGA,
                "ova" => MediaType.ANIME,
                _ => MediaType.ANIME
            };

            var media = await client.GetMediaAsync(query, type);

            if (media != null)
            {
                var score = $"{media.Score}%".Colorize(media.Score);

                return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"{IrcValues.BOLD}{media.EnglishTitle}{IrcValues.RESET} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} - {score} • {media.Url}"
                );
            }
            else
            {
                return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"Sorry, couldn't find that {TriggeringCommandName}."
                );
            }
        }

        [Command("waifu {query}")]
        [Command("husbando {query}")]
        public async Task<IClientMessage> GetCharacter(string query)
        {
            var character = await client.GetCharacterAsync(query);

            return new PrivateMessage(
                IncomingMessage.GetResponseTarget(),
                character == default
                ? $"Sorry, couldn't find that {TriggeringCommandName}."
                : $"{character.FirstName} {character.LastName} ({character.NativeName}) - {character.LargeImageUrl} | {character.SiteUrl}"
            );
        }
    }
}
