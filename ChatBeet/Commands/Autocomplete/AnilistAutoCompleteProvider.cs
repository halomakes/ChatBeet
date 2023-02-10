using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Miki.Anilist;
using Miki.Anilist.Objects;

namespace ChatBeet.Commands.Autocomplete;

public class AnilistAutoCompleteProvider : IAutocompleteProvider
{
    private const int MaxResults = 25;

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        if (ctx.FocusedOption.Value is string query && !string.IsNullOrWhiteSpace(query))
        {
            await using var scope = ctx.Services.CreateAsyncScope();
            var client = scope.ServiceProvider.GetRequiredService<AnilistClient>();
            var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

            return await cache.GetOrCreateAsync($"anilist:{ctx.FocusedOption.Name}:{query}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                if (ctx.FocusedOption.Name == "character")
                {
                    var results = await client.SearchCharactersAsync(query);
                    return results?
                        .Items
                        .Take(MaxResults)
                        .Select(BuildChoice);
                }
                else
                {
                    var results = await client.SearchMediaAsync(query);
                    return results?
                        .Items
                        .Take(MaxResults)
                        .Select(BuildChoice);
                }
            });
        }
        return Enumerable.Empty<DiscordAutoCompleteChoice>();
    }

    private DiscordAutoCompleteChoice BuildChoice(IMediaSearchResult media) => new((media.EnglishTitle ?? media.RomajiTitle ?? media.NativeTitle)?.Truncate(95), media.Id.ToString());

    private DiscordAutoCompleteChoice BuildChoice(ICharacterSearchResult @char) => new($"{@char.FirstName} {@char.LastName}"?.Truncate(95), @char.Id.ToString());
}
