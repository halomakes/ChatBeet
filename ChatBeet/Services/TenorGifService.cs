using ChatBeet.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tenor;
using Tenor.Schema;

namespace ChatBeet.Services;

public class TenorGifService
{
    private readonly TenorClient _client;
    private readonly IMemoryCache _cache;
    private readonly ChatBeetConfiguration.TenorConfiguration _config;

    public TenorGifService(IHttpClientFactory clientFactory, IMemoryCache cache, IOptions<ChatBeetConfiguration> options)
    {
        _cache = cache;
        _config = options.Value.Tenor;
        var settings = new TenorConfiguration
        {
            ApiKey = _config.ApiKey,
            ContentFilter = ContentFilter.Off,
            MediaFilter = MediaFilter.Minimal,
            Locale = ChatBeetConfiguration.Culture
        };
        _client = new TenorClient(settings, clientFactory.CreateClient());
    }

    public async Task<string> GetGifAsync(string search)
    {
        var result = await _cache.GetOrCreateAsync($"tenor:{search}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            return await _client.SearchAsync(search, limit: _config.QueryLimit);
        });
        var sequenceKey = $"tenor:seq:{search}";
        var seq = _cache.GetOrCreate(sequenceKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return -1;
        });
        _cache.Set(sequenceKey, ++seq);

        if (result?.Results?.Any() ?? false)
        {
            var options = result.Results
                .SelectMany(r => r.Media)
                .Where(m => m.ContainsKey(MediaType.Gif))
                .Select(m => m[MediaType.Gif])
                .Select(i => i.Url?.ToString())
                .ToList();
            var index = seq % options.Count();
            return options.ElementAtOrDefault(index);
        }
        return default;
    }
}