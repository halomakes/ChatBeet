using ChatBeet;
using DtellaRules.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PixivCS;
using PixivCS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class PixivRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly DtellaRuleConfiguration.PixivConfiguration pixivConfig;
        private readonly PixivAppAPI pixiv;
        private readonly IMemoryCache cache;

        public PixivRule(IOptions<ChatBeetConfiguration> options, IOptions<DtellaRuleConfiguration> dtlaOptions, PixivAppAPI pixiv, IMemoryCache cache)
        {
            config = options.Value;
            pixivConfig = dtlaOptions.Value.Pixiv;
            this.pixiv = pixiv;
            this.cache = cache;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(pixiv) (.*)");
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var search = match.Groups[2].Value;
                // use ID instead of name if provided
                var results = await cache.GetOrCreateAsync($"pixiv:{search}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    await pixiv.AuthAsync(pixivConfig.UserId, pixivConfig.Password);
                    return await pixiv.GetSearchIllustAsync(search);
                });

                var uri = PickUri(results);
                if (uri != null)
                    yield return new OutboundIrcMessage
                    {
                        Content = uri.ToString(),
                        Target = incomingMessage.Channel
                    };
                else
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Sorry, couldn't find that anything for {match.Groups[2].Value}, ya perv.",
                        Target = incomingMessage.Channel
                    };

                static Uri PickUri(SearchIllustResult searchResults)
                {
                    if (searchResults?.Illusts?.Any() ?? false)
                    {
                        var urls = searchResults.Illusts.PickRandom()?.ImageUrls;
                        return urls.Original ?? urls.Large ?? urls.Medium ?? urls.SquareMedium;
                    }
                    return null;
                }
            }
        }
    }
}
