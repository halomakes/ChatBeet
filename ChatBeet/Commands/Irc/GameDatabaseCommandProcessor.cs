using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using IGDB;
using IGDB.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc
{
    public class GameDatabaseCommandProcessor : CommandProcessor
    {
        private readonly IGDBClient client;
        private readonly IMemoryCache memoryCache;

        public GameDatabaseCommandProcessor(IGDBClient client, IMemoryCache memoryCache)
        {
            this.client = client;
            this.memoryCache = memoryCache;
        }

        [Command("game {mediaName}", Description = "Look up a title on IGDB.")]
        public async Task<IClientMessage> RespondAsync([Required] string mediaName)
        {
            var game = await memoryCache.GetOrCreateAsync($"igdb:{mediaName}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                var query = $@"fields name, platforms.abbreviation, aggregated_rating, first_release_date, url, genres.name, age_ratings.category, age_ratings.rating;
limit 4;
search ""{mediaName.Replace("\"", string.Empty)}"";";

                return (await client.QueryAsync<Game>(IGDBClient.Endpoints.Games, query))
                    .OrderByDescending(g => g.Name.Equals(mediaName, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();
            });

            if (game != null)
            {

                var messageBuilder = new StringBuilder($"{IrcValues.BOLD}{game.Name}{IrcValues.RESET}");
                if (game.FirstReleaseDate.HasValue)
                {
                    messageBuilder.Append($" ({game.FirstReleaseDate?.ToString("d MMM yyyy")})");
                }
                if (game.Platforms?.Values?.Any() ?? false)
                {
                    var platforms = string.Join(", ", game.Platforms.Values?.Select(p => p.Abbreviation));
                    messageBuilder.Append($" [{platforms}]");
                }
                var rating = game.AgeRatings?.Values?.FirstOrDefault(r => r.Category == AgeRatingCategory.ESRB);
                if (rating?.Rating != null)
                {
                    var color = rating.Rating switch
                    {
                        AgeRatingTitle.EC => IrcValues.TEAL,
                        AgeRatingTitle.E => IrcValues.GREEN,
                        AgeRatingTitle.E10 => IrcValues.LIME,
                        AgeRatingTitle.T => IrcValues.YELLOW,
                        AgeRatingTitle.M => IrcValues.ORANGE,
                        AgeRatingTitle.AO => IrcValues.RED,
                        _ => string.Empty
                    };
                    messageBuilder.Append($" {IrcValues.ITALIC}{color}Rated {rating.Rating}{IrcValues.RESET}");
                }
                if (game.AggregatedRating.HasValue)
                {
                    var score = $"{game.AggregatedRating?.ToString("0.00")}%".Colorize(game.AggregatedRating);
                    messageBuilder.Append($" - {score}");
                }
                if (game.Genres?.Values?.Any() ?? false)
                {
                    var platforms = string.Join(", ", game.Genres.Values?.Select(g => g.Name));
                    messageBuilder.Append($" • {platforms}");
                }
                messageBuilder.Append($" • More Info: {game.Url}");

                return new PrivateMessage(IncomingMessage.GetResponseTarget(), messageBuilder.ToString());
            }
            else
            {
                return new PrivateMessage(
                    IncomingMessage.GetResponseTarget(),
                    $"Sorry, couldn't find {IrcValues.ITALIC}{mediaName}{IrcValues.RESET}."
                );
            }
        }
    }
}
