using BooruSharp.Booru;
using BooruSharp.Search.Post;
using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class BooruService
    {
        private readonly IMemoryCache cache;
        private readonly Gelbooru gelbooru;
        private readonly ChatBeetConfiguration.BooruConfiguration booruConfig;

        public BooruService(IOptions<ChatBeetConfiguration> options, IMemoryCache cache, Gelbooru gelbooru)
        {
            this.cache = cache;
            this.gelbooru = gelbooru;
            booruConfig = options.Value.Booru;
        }

        public Task<string> GetRandomPostAsync(bool? safeContentOnly = true, params string[] tags) => GetRandomPostAsync(safeContentOnly, tags.AsEnumerable());

        public async Task<string> GetRandomPostAsync(bool? safeContentOnly = true, IEnumerable<string> tags = null)
        {
            var filter = safeContentOnly.HasValue ? (safeContentOnly.Value ? "rating:safe" : "-rating:safe") : string.Empty;
            var globalBlacklist = booruConfig.BlacklistedTags.Select(t => $"-{t}");

            var allTags = tags.Concat(globalBlacklist).Append(filter);

            var results = await cache.GetOrCreateAsync($"booru:{string.Join("|", allTags.OrderBy(t => t))}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                return await gelbooru.GetRandomPostsAsync(20, allTags.ToArray());
            });

            return PickImage(results, tags);

            static string PickImage(IEnumerable<SearchResult> searchResults, IEnumerable<string> searchTags)
            {
                if (searchResults?.Any() ?? false)
                {
                    var img = searchResults.PickRandom();
                    var rng = new Random();
                    var resultTags = img.tags
                        .Select(t => (MatchesInput: searchTags.Contains(t), Tag: t))
                        .OrderByDescending(p => p.MatchesInput)
                        .ThenBy(p => rng.Next())
                        .Select(p => p.MatchesInput ? $"{IrcValues.BOLD}{IrcValues.GREEN}{p.Tag}{IrcValues.RESET}" : p.Tag)
                        .Take(10)
                        .OrderBy(t => rng.Next());
                    var tagList = string.Join(", ", resultTags);
                    return $"{img.fileUrl} ({img.rating}) - {tagList}";
                }
                return default;
            }
        }
    }
}
