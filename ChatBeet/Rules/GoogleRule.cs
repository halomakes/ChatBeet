using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ChatBeet.Rules
{
    public class GoogleRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly HttpClient client;
        private readonly Regex rgx;
        private readonly IMemoryCache cache;

        public GoogleRule(IOptions<IrcBotConfiguration> options, IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            config = options.Value;
            client = clientFactory.CreateClient("noredirect");
            this.cache = cache;
            rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}(g|google|feelinglucky) (.*)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var searchTerm = match.Groups[2].Value.Trim();
                var uriBuilder = new UriBuilder($"https://www.google.com/search?btnI=1&q={WebUtility.UrlEncode(searchTerm)}");

                var resultLink = await TryGetTargetLink(uriBuilder.Uri);

                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From}: {resultLink}");
            }
        }

        private async Task<Uri> TryGetTargetLink(Uri feelingLuckyUri)
        {
            try
            {
                var result = await cache.GetOrCreateAsync($"google:{feelingLuckyUri}", async entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                    var page = await client.GetAsync(feelingLuckyUri);
                    return new GoogleResult
                    {
                        Uri = feelingLuckyUri,
                        RedirectUri = page.Headers.Location,
                        Body = await page.Content.ReadAsStringAsync()
                    };
                });

                if (result.RedirectUri != default && result.RedirectUri.AbsolutePath == "/url")
                {
                    var redirQuery = HttpUtility.ParseQueryString(result.RedirectUri.Query);
                    var targetPath = redirQuery["q"];
                    if (!string.IsNullOrEmpty(targetPath) && Uri.TryCreate(targetPath, UriKind.Absolute, out var targetUri))
                        return targetUri;
                }

                var links = Regex.Matches(result.Body, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);
                if (links.Any())
                {
                    var firstLink = links.FirstOrDefault().Value;
                    var hrefMatch = Regex.Match(firstLink, @"href=\""(.*?)\""", RegexOptions.Singleline);
                    if (hrefMatch.Success)
                    {
                        var hrefValue = hrefMatch.Groups[1].Value.Trim();
                        if (Uri.TryCreate(hrefValue, UriKind.Absolute, out var uri))
                            if (!uri.Host.Contains("google") && (uri.Scheme == "https" || uri.Scheme == "http"))
                                return uri;
                    }
                }

                return feelingLuckyUri;
            }
            catch (Exception)
            {
                return feelingLuckyUri;
            }
        }
        private struct GoogleResult
        {
            public Uri Uri;
            public string Body;
            public Uri RedirectUri;
        }
    }
}
