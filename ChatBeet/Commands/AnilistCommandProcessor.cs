using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Miki.Anilist;
using System.ComponentModel.DataAnnotations;
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

        [Command("anime {query}", Description = "Get information about an anime series from AniList")]
        [Command("manga {query}", Description = "Get infomration about a manga from AniList")]
        [Command("ln {query}", Description = "Get information about a light novel from AniList")]
        [Command("ova {query}", Description = "Get information about an anime OVA from AniList")]
        public async Task<IClientMessage> GetMedia([Required] string query)
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

        [Command("waifu {query}", Description = "Get information about a character from AniList")]
        [Command("husbando {query}", Description = "Get information about a character from AniList")]
        public async Task<IClientMessage> GetCharacter([Required] string query)
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
