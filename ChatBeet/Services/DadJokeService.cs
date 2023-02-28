using ChatBeet.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class DadJokeService
{
    private readonly HttpClient _client;

    public DadJokeService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
    }

    public async Task<string?> GetDadJokeAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://icanhazdadjoke.com/")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        if (!response.IsSuccessStatusCode)
            return null;
        await using var contentStream = await response.Content.ReadAsStreamAsync();
        return DeserializeJsonFromStream<DadJoke>(contentStream)!.Joke;
    }

    private static T? DeserializeJsonFromStream<T>(Stream? stream)
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