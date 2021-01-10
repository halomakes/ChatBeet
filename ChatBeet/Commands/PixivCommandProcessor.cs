using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PixivCS;
using PixivCS.Objects;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class PixivCommandProcessor : CommandProcessor
    {
        private readonly ChatBeetConfiguration.PixivConfiguration pixivConfig;
        private readonly PixivAppAPI pixiv;
        private readonly IMemoryCache cache;

        public PixivCommandProcessor(IOptions<ChatBeetConfiguration> dtlaOptions, PixivAppAPI pixiv, IMemoryCache cache)
        {
            pixivConfig = dtlaOptions.Value.Pixiv;
            this.pixiv = pixiv;
            this.cache = cache;
        }

        [Command("pixiv {query}", Description = "Get a random artwork from Pixiv.")]
        public async Task<IClientMessage> RespondAsync([Required] string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var results = await cache.GetOrCreateAsync($"pixiv:{query}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    await pixiv.AuthAsync(pixivConfig.UserId, pixivConfig.Password);
                    return await pixiv.GetSearchIllustAsync(query);
                });

                var text = PickImage(results);
                if (text != null)
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), text);
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything for {query}, ya perv.");
                }


                static string PickImage(SearchIllustResult searchResults)
                {
                    if (searchResults?.Illusts?.Any() ?? false)
                    {
                        var img = searchResults.Illusts.PickRandom();
                        return $"{IrcValues.BOLD}{img.Title}{IrcValues.RESET} by {IrcValues.BOLD}{img.User?.Name}{IrcValues.RESET} - https://www.pixiv.net/en/artworks/{img.Id}";
                    }
                    return null;
                }
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Please provide a search term.");
            }
        }
    }
}
