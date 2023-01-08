using ChatBeet.Commands.Discord.Autocomplete;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using Miki.Anilist;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class AnilistCommandModule : ApplicationCommandModule
{
    private readonly AnilistService client;

    public AnilistCommandModule(AnilistService client)
    {
        this.client = client;
    }

    [SlashCommand("anime", "Get information about an anime series from AniList")]
    public Task GetAnime(InteractionContext ctx, [Option("media", "Anime to search for"), Autocomplete(typeof(AnilistAutoCompleteProvider))] string query) => GetMedia(ctx, query, MediaType.ANIME);


    [SlashCommand("manga", "Get information about a manga from AniList")]
    public Task GetManga(InteractionContext ctx, [Option("media", "Manga to search for"), Autocomplete(typeof(AnilistAutoCompleteProvider))] string query) => GetMedia(ctx, query, MediaType.MANGA);


    private async Task GetMedia(InteractionContext ctx, string query, MediaType type)
    {
        var media = await client.GetMediaAsync(query, type);

        if (media is not null)
        {
            var description = media.Description.RemoveSpoilers().Truncate(1000);

            var embed = new DiscordEmbedBuilder
            {
                ImageUrl = media.CoverImage,
                Url = media.Url,
                Description = description,
                Title = media.EnglishTitle
            };
            var text = @$"{Formatter.Bold(media.EnglishTitle)} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} - {media.Score}%
{description}
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
    public async Task GetCharacter(InteractionContext ctx, [Option("character", "Name of the character to search for"), Autocomplete(typeof(AnilistAutoCompleteProvider))] string query)
    {
        var character = await client.GetCharacterAsync(query);

        if (character is not null)
        {
            var description = character.Description.RemoveSpoilers().Truncate(1000);
            var embed = new DiscordEmbedBuilder
            {
                ImageUrl = character.LargeImageUrl,
                Url = character.SiteUrl,
                Description = $"{character.FirstName} {character.LastName}"
            };
            var fullName = Formatter.Bold($"{character.FirstName} {character.LastName}");
            var text = @$"{fullName} ({character.NativeName})
{description}
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
