using Miki.Anilist;
using Miki.Anilist.Objects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DtellaRules.Services
{
    public class AnilistService
    {
        private readonly AnilistClient client;

        public AnilistService(AnilistClient client)
        {
            this.client = client;
        }

        public Task<IMedia> GetMediaAsync(string search) => int.TryParse(search, out var id)
            ? GetMediaAsync(id)
            : GetTopSearchItemAsync(search, t => client.SearchMediaAsync(t), r => client.GetMediaAsync(r.Id));

        public Task<IMedia> GetMediaAsync(int id) => client.GetMediaAsync(id);

        public Task<ICharacter> GetCharacterAsync(string search) => long.TryParse(search, out var id)
            ? GetCharacterAsync(id)
            : GetTopSearchItemAsync(search, t => client.SearchCharactersAsync(t), r => client.GetCharacterAsync(r.Id));

        public Task<ICharacter> GetCharacterAsync(long id) => client.GetCharacterAsync(id);

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
}
