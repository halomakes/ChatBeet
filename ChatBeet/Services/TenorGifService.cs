using ChatBeet.Configuration;
using ChatBeet.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tenor;
using Tenor.Schema;

namespace ChatBeet.Services
{
    public class TenorGifService
    {
        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;
        private readonly ChatBeetConfiguration.TenorConfiguration config;

        public TenorGifService(IHttpClientFactory clientFactory, IMemoryCache cache, IOptions<ChatBeetConfiguration> options)
        {
            httpClient = clientFactory.CreateClient();
            this.cache = cache;
            config = options.Value.Tenor;
        }

        public async Task<string> GetGifAsync(string search)
        {
            var result = await cache.GetOrCreateAsync($"tenor:{search}", async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                var client = new TenorClient(config.ApiKey, httpClient);
                return await client.SearchAsync(search, limit: config.QueryLimit);
            });

            return result?.Results?.Any() ?? false
                ? result.Results
                    .SelectMany(r => r.Media)
                    .Where(m => m.ContainsKey(MediaType.Gif))
                    .Select(m => m[MediaType.Gif])
                    .Select(i => i.Url?.ToString())
                    .PickRandom()
                : null;
        }
    }
}
