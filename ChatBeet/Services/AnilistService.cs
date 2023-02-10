using Miki.Anilist;
using Miki.Anilist.Objects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class AnilistService
{
    private readonly AnilistClient _client;

    public AnilistService(AnilistClient client)
    {
        _client = client;
    }

    public Task<IMedia> GetMediaAsync(string search, MediaType format = MediaType.ANIME) => int.TryParse(search, out var id)
        ? GetMediaAsync(id)
        : GetTopSearchItemAsync(search, t => _client.SearchMediaAsync(t, type: format), r => _client.GetMediaAsync(r.Id));

    public Task<IMedia> GetMediaAsync(int id) => _client.GetMediaAsync(id);

    public Task<ICharacter> GetCharacterAsync(string search) => long.TryParse(search, out var id)
        ? GetCharacterAsync(id)
        : GetTopSearchItemAsync(search, t => _client.SearchCharactersAsync(t), r => _client.GetCharacterAsync(r.Id));

    public Task<ICharacter> GetCharacterAsync(long id) => _client.GetCharacterAsync(id);

    private async Task<TResult> GetTopSearchItemAsync<TResult, TSearch>(string keywords, Func<string, Task<ISearchResult<TSearch>>> search, Func<TSearch, Task<TResult>> fetch)
    {
        var searchResults = await search(keywords);
        var topResult = searchResults.Items.FirstOrDefault();
        if (topResult != null)
        {
            return await fetch(topResult);
        }

        return default;
    }
}