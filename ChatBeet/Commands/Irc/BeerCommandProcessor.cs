using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Untappd.Client;

namespace ChatBeet.Commands.Irc
{
    public class BeerCommandProcessor : CommandProcessor
    {
        private readonly UntappdClient client;
        private readonly IMemoryCache cache;

        public BeerCommandProcessor(UntappdClient client, IMemoryCache cache)
        {
            this.client = client;
            this.cache = cache;
        }

        [Command("beer {beerName}", Description = "Get info about a beer on Untappd.")]
        public async Task<IClientMessage> FindBeerAsync([Required] string beerName)
        {
            var results = await cache.GetOrCreateAsync($"beer:{beerName}", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(3);
                return client.SearchBeers(beerName);
            });

            if (results?.Response?.Beers?.Items?.Any() ?? false)
            {
                var beer = results.Response.Beers.Items.FirstOrDefault();
                var desc = $"{IrcValues.BOLD}{beer.Beer.BeerName}{IrcValues.RESET}{(beer.Beer.InProduction > 0 ? string.Empty : " (Out of Production)")} from {beer.Brewery.BreweryName}";
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), string.IsNullOrEmpty(beer.Beer.BeerDescription) ? desc : $"{desc} - {beer.Beer.BeerDescription}");
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Sorry, couldn't find that beer.");
            }
        }
    }
}
