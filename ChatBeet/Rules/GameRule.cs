using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using IGDB;
using IGDB.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class GameRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly IGDBApi client;
        private readonly IMemoryCache memoryCache;
        private readonly Regex filter;

        public GameRule(IOptions<IrcBotConfiguration> options, IGDBApi client, IMemoryCache memoryCache)
        {
            config = options.Value;
            this.client = client;
            this.memoryCache = memoryCache;
            filter = new Regex($"^{Regex.Escape(config.CommandPrefix)}(game) (.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => filter.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                var mediaName = match.Groups[2].Value.Trim();

                var game = await memoryCache.GetOrCreateAsync($"igdb:{mediaName}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                    var query = $@"fields name, platforms.abbreviation, aggregated_rating, cover.url, collection.name, first_release_date, summary, url;
limit 1;
search ""{mediaName.Replace("\"", string.Empty)}"";";

                    return (await client.QueryAsync<Game>(Client.Endpoints.Games, query)).FirstOrDefault();
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
                    if (game.AggregatedRating.HasValue)
                    {
                        var score = $"{game.AggregatedRating?.ToString("0.00")}%".Colorize(game.AggregatedRating);
                        messageBuilder.Append($"- {score}");
                    }
                    if (game.Cover?.Value?.Url != null)
                    {
                        var cover = game.Cover?.Value?.Url?.Replace("t_thumb", "t_1080p");
                        messageBuilder.Append($" https:{cover}");
                    }
                    messageBuilder.Append($" More Info: {game.Url}");

                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), messageBuilder.ToString());
                }
                else
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"Sorry, couldn't find {match.Groups[1].Value}."
                    );
                }
            }
        }
    }
}