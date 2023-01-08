using ChatBeet.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord.Autocomplete;

public class BooruTagAutocompleteProvider : IAutocompleteProvider
{
    private const int MaxResults = 25;

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        if (ctx.FocusedOption.Value is string query && !string.IsNullOrWhiteSpace(query) && !query.Contains(' '))
        {
            await using var scope = ctx.Services.CreateAsyncScope();
            var booru = scope.ServiceProvider.GetRequiredService<BooruService>();

            var tags = await booru.GetTagsAsync(query);
            return tags
                .Take(MaxResults)
                .Select(t => new DiscordAutoCompleteChoice(t, t));
        }
        return Enumerable.Empty<DiscordAutoCompleteChoice>();
    }
}
