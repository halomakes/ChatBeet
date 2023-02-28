using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class ComplimentService
{
    private readonly HttpClient _client;

    public ComplimentService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> GetComplimentAsync()
    {
        var result = await _client.GetAsync("https://complimentr.com/api");
        var content = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await result.Content.ReadAsStreamAsync());
        return content!["compliment"];
    }
}