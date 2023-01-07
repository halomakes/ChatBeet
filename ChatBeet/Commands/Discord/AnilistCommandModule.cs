using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Miki.Anilist;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord
{
    [ModuleLifespan(ModuleLifespan.Transient)]
    public class AnilistCommandModule : BaseCommandModule
    {
        private readonly AnilistService client;

        public AnilistCommandModule(AnilistService client)
        {
            this.client = client;
        }

        [Command("anime"), Description("Get information about an anime series from AniList")]
        [SlashCommand("anime", "Get information about an anime series from AniList")]
        public Task GetAnime(CommandContext ctx, [RemainingText] string query) => GetMedia(ctx, query, MediaType.ANIME);


        [Command("manga"), Description("Get infomration about a manga from AniList")]
        [SlashCommand("manga", "Get information about a manga from AniList")]
        public Task GetManga(CommandContext ctx, [RemainingText] string query) => GetMedia(ctx, query, MediaType.MANGA);


        private async Task GetMedia(CommandContext ctx, string query, MediaType type)
        {
            var media = await client.GetMediaAsync(query, type);

            if (media is not null)
            {
                var embed = new DiscordEmbedBuilder
                {
                    ImageUrl = media.CoverImage,
                    Url = media.Url,
                    Description = media.Description,
                    Title = media.EnglishTitle
                };
                var text = $"{Formatter.Bold(media.EnglishTitle)} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} - {media.Score}%";

                await ctx.RespondAsync(text, embed);
            }
            else
            {
                await ctx.RespondAsync($"Sorry, couldn't find that {type.ToString().ToLower()}.");
            }
        }

        [Command("waifu"), Description("Get information about a character from AniList")]
        [Aliases("husbando")]
        [SlashCommand("waifu", "Get information about a character from AniList")]
        public async Task GetCharacter(CommandContext ctx, [RemainingText] string query)
        {
            var character = await client.GetCharacterAsync(query);

            if (character is not null)
            {
                var embed = new DiscordEmbedBuilder
                {
                    ImageUrl = character.LargeImageUrl,
                    Url = character.SiteUrl,
                    Description = $"{character.FirstName} {character.LastName}"
                };
                var text = $"{character.FirstName} {character.LastName} ({character.NativeName}) {Environment.NewLine}{character.Description}";

                await ctx.RespondAsync(text, embed);
            }
            else
            {
                await ctx.RespondAsync($"Sorry, couldn't find that character.");
            }
        }
    }
}
