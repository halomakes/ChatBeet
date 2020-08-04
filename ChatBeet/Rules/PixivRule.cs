using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PixivCS;
using PixivCS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class PixivRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly ChatBeetConfiguration.PixivConfiguration pixivConfig;
        private readonly PixivAppAPI pixiv;
        private readonly IMemoryCache cache;
        private readonly Regex rgx;

        public PixivRule(IOptions<IrcBotConfiguration> options, IOptions<ChatBeetConfiguration> dtlaOptions, PixivAppAPI pixiv, IMemoryCache cache)
        {
            config = options.Value;
            pixivConfig = dtlaOptions.Value.Pixiv;
            this.pixiv = pixiv;
            this.cache = cache;
            rgx = new Regex($"^{config.CommandPrefix}(pixiv) (.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var search = match.Groups[2].Value;

                var results = await cache.GetOrCreateAsync($"pixiv:{search}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    await pixiv.AuthAsync(pixivConfig.UserId, pixivConfig.Password);
                    return await pixiv.GetSearchIllustAsync(search);
                });

                var text = PickImage(results);
                if (text != null)
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), text);
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Sorry, couldn't find that anything for {search}, ya perv.");
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
        }
    }
}
