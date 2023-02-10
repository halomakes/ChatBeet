using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Caching.Memory;
using Miki.UrbanDictionary;
using Miki.UrbanDictionary.Objects;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class UrbanDictionaryCommandModule : ApplicationCommandModule
{
    private readonly IMemoryCache _cache;
    private readonly UrbanDictionaryApi _api;

    public UrbanDictionaryCommandModule(IMemoryCache cache, UrbanDictionaryApi api)
    {
        _cache = cache;
        _api = api;
    }

    [SlashCommand("udict", "Look up a term on Urban Dictionary")]
    public async Task FindDefinition(InteractionContext ctx, [Option("term", "Term to search for")] string term)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var results = await _cache.GetOrCreateAsync($"udict:{term}", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return (await _api.SearchTermAsync(term))?.List;
        });

        if (results is null || !results.Any())
        {
            // nothing
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Sorry, couldn't find a definition for \"{term}\""));
        }
        else if (results.Count == 1)
        {
            // single
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent(BuildContent(results.First())));
        }
        else
        {
            //multiple
            var pages = results.Select(r => new Page(BuildContent(r))).ToList();
            var message = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent("Mulitple results found.")
                .AsEphemeral());
            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, PaginationBehaviour.WrapAround, ButtonPaginationBehavior.DeleteButtons);
        }

        string BuildContent(UrbanDictionaryEntry entry) => @$"{Formatter.Bold(entry.Term)}
{entry.Definition}
{Formatter.Italic(entry.Example)}
👍 {entry.ThumbsUp}   👎 {entry.ThumbsDown}";
    }
}
