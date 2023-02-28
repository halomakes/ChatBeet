using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using IGDB;
using IGDB.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class GameLookupCommandModule : ApplicationCommandModule
{
    private readonly IGDBClient _client;
    private readonly IMemoryCache _memoryCache;

    public GameLookupCommandModule(IGDBClient client, IMemoryCache memoryCache)
    {
        _client = client;
        _memoryCache = memoryCache;
    }

    [SlashCommand("game", "Look up a title on IGDB")]
    public async Task RespondAsync(InteractionContext ctx, [Option("name", "Name of game to search for")] string mediaName)
    {
        var game = await _memoryCache.GetOrCreateAsync($"igdb:{mediaName}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

            var query = $@"fields name, platforms.abbreviation, aggregated_rating, first_release_date, url, genres.name, age_ratings.category, age_ratings.rating;
limit 4;
search ""{mediaName.Replace("\"", string.Empty)}"";";

            return (await _client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query))
                .OrderByDescending(g => g.Name.Equals(mediaName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
        });

        if (game != null)
        {
            var messageBuilder = new StringBuilder(Formatter.Bold(game.Name));
            if (game.FirstReleaseDate.HasValue)
            {
                messageBuilder.Append($" ({game.FirstReleaseDate?.ToString("d MMM yyyy")})");
            }
            if (game.Platforms?.Values?.Any() ?? false)
            {
                var platforms = string.Join(", ", game.Platforms.Values?.Select(p => p.Abbreviation) ?? Array.Empty<string>());
                messageBuilder.Append($" [{platforms}]");
            }
            var rating = game.AgeRatings?.Values?.FirstOrDefault(r => r.Category == AgeRatingCategory.ESRB);
            if (rating?.Rating != null)
            {
                messageBuilder.Append($" {Formatter.Italic($"Rated {rating.Rating}")}");
            }
            if (game.AggregatedRating.HasValue)
            {
                var score = $"{game.AggregatedRating?.ToString("0.00")}%";
                messageBuilder.Append($" - {score}");
            }
            if (game.Genres?.Values?.Any() ?? false)
            {
                var platforms = string.Join(", ", game.Genres.Values?.Select(g => g.Name) ?? Array.Empty<string>());
                messageBuilder.Append($" • {platforms}");
            }
            messageBuilder.Append($" • {game.Url}");

            var responseBuilder = new DiscordInteractionResponseBuilder()
                .WithContent($"{messageBuilder}{Environment.NewLine}{game.Summary}");

            if (!string.IsNullOrEmpty(game.Cover?.Value?.Url))
                responseBuilder = responseBuilder
                    .AddEmbed(new DiscordEmbedBuilder
                    {
                        ImageUrl = game.Cover.Value.Url,
                        Title = game.Name
                    });

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, responseBuilder);
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find {Formatter.Italic(mediaName)}."));
        }
    }
}
