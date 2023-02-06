using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ChatBeet.Configuration;
using ChatBeet.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ChatBeet.Services;

public class MemeService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public MemeService(IHttpClientFactory clientFactory, IMemoryCache cache, IOptions<ChatBeetConfiguration> options)
    {
        _cache = cache;
        var baseurl = options.Value.Urls["Memes"];
        _client = clientFactory.CreateClient("Memes");
        _client.BaseAddress = new(baseurl);
        _client.DefaultRequestHeaders.Accept.Add(new("application/json"));
    }

    public async Task<string> GetRandomImageAsync(string query) => (await GetImagesAsync(query)).PickRandom();

    private async Task<List<string>> GetImagesAsync(string query) => await _cache.GetOrCreateAsync($"memes:{query}", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
        var content = await _client.GetFromJsonAsync<ResponseWrapper>($"/api/posts?query=sort:random {query}", new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return content.Results.Select(r => $"{_client.BaseAddress}/{r.ContentUrl}").ToList();
    });

    internal record ResponseWrapper(List<Result> Results);

    internal record Result(string ContentUrl);
}
