using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Meowtrix.PixivApi;
using Meowtrix.PixivApi.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class PixivCommandProcessor : CommandProcessor
    {
        private readonly ChatBeetConfiguration.PixivConfiguration pixivConfig;
        private readonly PixivApiClient pixiv;
        private readonly IMemoryCache cache;

        public PixivCommandProcessor(IOptions<ChatBeetConfiguration> dtlaOptions, PixivApiClient pixiv, IMemoryCache cache)
        {
            pixivConfig = dtlaOptions.Value.Pixiv;
            this.pixiv = pixiv;
            this.cache = cache;
            this.pixiv.DefaultRequestHeaders.AcceptLanguage.Add(new(ChatBeetConfiguration.Culture.Name));

        }

        [Command("pixiv {query}", Description = "Get a random artwork from Pixiv.")]
        public async Task<IClientMessage> RespondAsync([Required] string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var results = await cache.GetOrCreateAsync($"pixiv:{query}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    var (authTime, authResponse) = await pixiv.AuthAsync(pixivConfig.UserId, pixivConfig.Password);
                    return await pixiv.SearchIllustsAsync(authToken: authResponse.AccessToken, word: query);
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
                    if (searchResults?.Illusts.Any() ?? false)
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
