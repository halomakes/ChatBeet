using ChatBeet.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class DadJokeService
{
    private readonly HttpClient _client;
    private readonly IMemoryCache _cache;

    public DadJokeService(IHttpClientFactory clientFactory, IMemoryCache cache)
    {
        _client = clientFactory.CreateClient();
        _cache = cache;
    }

    public async Task<string> GetDadJokeAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://icanhazdadjoke.com/")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        if (response.IsSuccessStatusCode)
        {
            using var contentStream = await response.Content.ReadAsStreamAsync();
            return DeserializeJsonFromStream<DadJoke>(contentStream)?.Joke;
        }

        return null;
    }

    private static T DeserializeJsonFromStream<T>(Stream stream)
    {
        if (stream == null || stream.CanRead == false)
            return default(T);

        using (var sr = new StreamReader(stream))
        using (var jtr = new JsonTextReader(sr))
        {
            var js = new JsonSerializer();
            var searchResult = js.Deserialize<T>(jtr);
            return searchResult;
        }
    }
}