using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Commands.Autocomplete;

public class MetadataAutocompleteProvider : IAutocompleteProvider
{
    private const int MaxResults = 25;

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        if (ctx.FocusedOption.Name != "tag")
            return Enumerable.Empty<DiscordAutoCompleteChoice>();
        await using var scope = ctx.Services.CreateAsyncScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var searchValue = (ctx.FocusedOption.Value as string)?.ToLower();
        var tags = await userRepo.Metadata
            .Where(m => m.GuildId == ctx.Guild.Id)
            .Select(m => m.Key)
            .Where(k => string.IsNullOrWhiteSpace(searchValue) || k.StartsWith(searchValue))
            .Distinct()
            .Take(MaxResults)
            .ToListAsync();
        return tags.Select(t => new DiscordAutoCompleteChoice(t, t));
    }
}