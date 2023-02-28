using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ChatBeet.Services;

public class GoogleSearchService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public GoogleSearchService(IHttpClientFactory clientFactory, IMemoryCache cache)
    {
        _client = clientFactory.CreateClient("no-redirect");
        _cache = cache;
    }

    public async Task<Uri> GetFeelingLuckyResultAsync(string query)
    {
        var feelingLuckyUri = new Uri($"https://www.google.com/search?btnI=1&q={WebUtility.UrlEncode(query)}");
        try
        {
            var result = await _cache.GetOrCreateAsync($"google:{feelingLuckyUri}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                var page = await _client.GetAsync(feelingLuckyUri);
                return new GoogleResult
                {
                    Uri = feelingLuckyUri,
                    RedirectUri = page.Headers.Location!,
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
        public string? Body;
        public Uri? RedirectUri;
    }
}