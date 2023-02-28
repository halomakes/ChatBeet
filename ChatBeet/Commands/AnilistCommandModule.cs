using System.Text;
using System.Threading.Tasks;
using ChatBeet.Commands.Autocomplete;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using Miki.Anilist;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class AnilistCommandModule : ApplicationCommandModule
{
    private readonly AnilistService _client;

    public AnilistCommandModule(AnilistService client)
    {
        _client = client;
    }

    [SlashCommand("anime", "Get information about an anime series from AniList")]
    public Task GetAnime(InteractionContext ctx, [Option("media", "Anime to search for"), Autocomplete(typeof(AnilistAutoCompleteProvider))] string query) => GetMedia(ctx, query, MediaType.ANIME);


    [SlashCommand("manga", "Get information about a manga from AniList")]
    public Task GetManga(InteractionContext ctx, [Option("media", "Manga to search for"), Autocomplete(typeof(AnilistAutoCompleteProvider))] string query) => GetMedia(ctx, query, MediaType.MANGA);


    private async Task GetMedia(InteractionContext ctx, string query, MediaType type)
    {
        var media = await _client.GetMediaAsync(query, type);

        if (media is not null)
        {
            var description = media.Description?.Split(Environment.NewLine).FirstOrDefault()?.RemoveSpoilers().Truncate(250);

            var embed = new DiscordEmbedBuilder
            {
                ImageUrl = media.CoverImage,
                Url = media.Url,
                Description = description,
                Title = media.EnglishTitle
            };
            var text = new StringBuilder(@$"{Formatter.Bold(media.EnglishTitle)} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Score}%
{media.Status}");
            if (media.Episodes is not null)
                text.Append($" - {media.Episodes} {(media.Episodes == 1 ? "episode" : "episodes")}");
            if (media.Volumes is not null)
                text.Append($" - {media.Volumes} {(media.Volumes == 1 ? "volume" : "volumes")}");
            if (media.Chapters is not null)
                text.Append($" - {media.Chapters} {(media.Chapters == 1 ? "chapter" : "chapters")}");
            text.AppendLine();
            if (media.Genres is not null && media.Genres.Any())
                text.AppendLine(string.Join(", ", media.Genres.Take(7)));
            text.AppendLine(Formatter.MaskedUrl("View on AniList", new Uri(media.Url)));

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(text.ToString())
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
        var character = await _client.GetCharacterAsync(query);

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

    [SlashCommand("husbando", "Get information about a character from AniList")]
    public Task GetCharacterAlias(InteractionContext ctx, [Option("character", "Name of the character to search for"), Autocomplete(typeof(AnilistAutoCompleteProvider))] string query)
        => GetCharacter(ctx, query);
}
