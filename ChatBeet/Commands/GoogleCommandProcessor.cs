using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using OpenGraphNet;
using System;
using System.Collections.Generic;
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

        [Command("google {query}", Description = "Look something up on Google using I'm Feeling Lucky")]
        [Command("feelinglucky {query}", Description = "Look something up on Google using I'm Feeling Lucky")]
        public async IAsyncEnumerable<IClientMessage> Search(string query)
        {
            var uriBuilder = new UriBuilder($"https://www.google.com/search?btnI=1&q={WebUtility.UrlEncode(query)}");
            var resultLink = await TryGetTargetLink(uriBuilder.Uri);
            if (!resultLink.Host.Contains("google.com", StringComparison.OrdinalIgnoreCase))
            {
                // not a google link, try to generate preview
                var metaTask = OpenGraph.ParseUrlAsync(resultLink);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));
                await Task.WhenAny(metaTask, timeoutTask);

                if (metaTask.IsCompleted)
                {
                    var meta = metaTask.Result;
                    yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink} {meta.ToIrcSummary()}");
                }
                else
                {
                    // pinging page is taking too long, go ahead and give url then follow up with metadata later
                    yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink}");
                    var meta = await metaTask;
                    yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), meta.ToIrcSummary(maxDescriptionLength: 80));
                }
            }
            else
            {
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IncomingMessage.From}: {resultLink}");
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
