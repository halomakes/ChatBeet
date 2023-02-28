using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Commands.Autocomplete;

public class BooruTagAutocompleteProvider : IAutocompleteProvider
{
    private const int MaxResults = 25;

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
    {
        if (ctx.FocusedOption.Value is string query && !query.Contains(' '))
        {
            await using var scope = ctx.Services.CreateAsyncScope();
            if (string.IsNullOrEmpty(query))
            {
                // get top tags for user
                var db = scope.ServiceProvider.GetRequiredService<IBooruRepository>();
                var users = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
                var currentUser = await users.Users.FirstOrDefaultAsync(u => u.Discord!.Id == ctx.User.Id);
                if (currentUser is not null)
                {
                    var topTags = await db.TagHistories
                        .Where(th => th.UserId == currentUser.Id)
                        .GroupBy(th => th.Tag)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .Take(MaxResults)
                        .ToListAsync();
                    return topTags
                        .Select(t => new DiscordAutoCompleteChoice(t, t));
                }
            }
            else
            {
                // autocomplete
                var booru = scope.ServiceProvider.GetRequiredService<BooruService>();

                var tags = await booru.GetTagsAsync(query);
                return tags
                    .Take(MaxResults)
                    .Select(t => new DiscordAutoCompleteChoice(t, t));
            }
        }

        return Enumerable.Empty<DiscordAutoCompleteChoice>();
    }
}