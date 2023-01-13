using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;
using Untappd.Client;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BeerCommandModule : ApplicationCommandModule
{
    private readonly UntappdClient client;
    private readonly IMemoryCache cache;

    public BeerCommandModule(UntappdClient client, IMemoryCache cache)
    {
        this.client = client;
        this.cache = cache;
    }

    [SlashCommand("beer", "Get info about a beer on Untappd")]
    public async Task FindBeerAsync(InteractionContext ctx, [Option("beer-name", "Name of the beer to get info about")] string beerName)
    {
        var results = await cache.GetOrCreateAsync($"beer:{beerName}", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(3);
            return client.SearchBeers(beerName);
        });

        if (results?.Response?.Beers?.Items?.Any() ?? false)
        {
            var beer = results.Response.Beers.Items.FirstOrDefault();
            var text = @$"{Formatter.Bold(beer.Beer.BeerName)}{(beer.Beer.InProduction > 0 ? string.Empty : " (Out of Production)")} from {beer.Brewery.BreweryName}
{beer.Beer.BeerDescription}";

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(text)
                );
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find that beer.")
                );
        }
    }
}
