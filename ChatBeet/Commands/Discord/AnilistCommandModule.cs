using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Miki.Anilist;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord
{
    [SlashModuleLifespan(SlashModuleLifespan.Scoped)]
    public class AnilistCommandModule : ApplicationCommandModule
    {
        private readonly AnilistService client;

        public AnilistCommandModule(AnilistService client)
        {
            this.client = client;
        }

        [SlashCommand("anime", "Get information about an anime series from AniList")]
        public Task GetAnime(InteractionContext ctx, [Option("query", "Anime to search for")] string query) => GetMedia(ctx, query, MediaType.ANIME);


        [SlashCommand("manga", "Get information about a manga from AniList")]
        public Task GetManga(InteractionContext ctx, [Option("query", "Manga to search for")] string query) => GetMedia(ctx, query, MediaType.MANGA);


        private async Task GetMedia(InteractionContext ctx, string query, MediaType type)
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
                var text = @$"{Formatter.Bold(media.EnglishTitle)} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} - {media.Score}%
{media.Description.RemoveSpoilers()}
{Formatter.MaskedUrl("View on AniList", new Uri(media.Url))}";

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(text)
                    .AddEmbed(embed)
                    );
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"Sorry, couldn't find that {type.ToString().ToLower()}.")
                    );
            }
        }

        [SlashCommand("waifu", "Get information about a character from AniList")]
        public async Task GetCharacter(InteractionContext ctx, [Option("query", "Name of the character to search for")] string query)
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
                var fullName = Formatter.Bold($"{character.FirstName} {character.LastName}");
                var text = @$"{fullName} ({character.NativeName})
{character.Description.RemoveSpoilers()}
{Formatter.MaskedUrl("View on AniList", new Uri(character.SiteUrl))}";

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(text)
                    .AddEmbed(embed)
                    );
            }
            else
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"Sorry, couldn't find that character.")
                    );
            }
        }
    }
}
