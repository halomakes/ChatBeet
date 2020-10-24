using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ChatBeet.Commands
{
    public class GoogleCommandProcessor : CommandProcessor
    {
        private readonly HttpClient client;
        private readonly IMemoryCache cache;

        public GoogleCommandProcessor(IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            client = clientFactory.CreateClient("noredirect");
            this.cache = cache;
        }

        [Command("google {query}")]
        [Command("feelinglucky {query}")]
        public async Task<IClientMessage> Search(string query)
        {
            var uriBuilder = new UriBuilder($"https://www.google.com/search?btnI=1&q={WebUtility.UrlEncode(query)}");
            var resultLink = await TryGetTargetLink(uriBuilder.Uri);

            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink}");
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
