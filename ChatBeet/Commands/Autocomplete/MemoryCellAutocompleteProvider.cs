using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Commands.Autocomplete;

public class MemoryCellAutocompleteProvider : IAutocompleteProvider
{
    private const int MaxResults = 25;

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        await using var scope = ctx.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IDefinitionsRepository>();
        if (ctx.FocusedOption.Value is null || (ctx.FocusedOption.Value is string emptyValue && string.IsNullOrWhiteSpace(emptyValue)))
        {
            var cells = await dbContext.Definitions
                .Where(r => r.GuildId == ctx.Guild.Id)
                .OrderBy(r => r.Key)
                .Take(MaxResults)
                .ToListAsync();
            return cells.Select(BuildChoice);
        }
        else if (ctx.FocusedOption.Value is string currentValue)
        {
            var asLower = currentValue.ToLower();
            var cells = await dbContext.Definitions
                .Where(r => r.GuildId == ctx.Guild.Id)
                .Select(c => new
                {
                    Item = c,
                    Rating = (c.Key.ToLower().StartsWith(asLower) ? 5 : 0)
                             + (c.Key.ToLower().Contains(asLower) ? 4 : 0)
                             + (c.Value!.ToLower().StartsWith(asLower) ? 2 : 0)
                             + (c.Value.ToLower().Contains(asLower) ? 1 : 0)
                })
                .Where(r => r.Rating > 0)
                .OrderByDescending(r => r.Rating)
                .ThenBy(r => r.Item.Key)
                .Select(r => r.Item)
                .Take(MaxResults)
                .ToListAsync();
            return cells.Select(BuildChoice);
        }
        else
        {
            return Enumerable.Empty<DiscordAutoCompleteChoice>();
        }
    }

    private DiscordAutoCompleteChoice BuildChoice(Definition cell) => new(cell.Key, cell.Key);
}