using BooruSharp.Booru;
using BooruSharp.Search.Post;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using GravyBot;
using Microsoft.EntityFrameworkCore;
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
        private readonly BooruContext context;

        public BooruService(IOptions<ChatBeetConfiguration> options, IMemoryCache cache, Gelbooru gelbooru, BooruContext context)
        {
            this.cache = cache;
            this.gelbooru = gelbooru;
            booruConfig = options.Value.Booru;
            this.context = context;
        }

        public Task<string> GetRandomPostAsync(bool? safeContentOnly, string requestor, params string[] tags) => GetRandomPostAsync(safeContentOnly, requestor, tags.AsEnumerable());

        public async Task<string> GetRandomPostAsync(bool? safeContentOnly, string requestor, IEnumerable<string> tags = null)
        {
            var filter = safeContentOnly.HasValue ? (safeContentOnly.Value ? "rating:safe" : "-rating:safe") : string.Empty;
            var globalBlacklist = Negate(booruConfig.BlacklistedTags);
            var userBlacklist = string.IsNullOrEmpty(requestor)
                ? new List<string>()
                : Negate(await GetBlacklistedTags(requestor));

            var allTags = tags.Concat(globalBlacklist).Concat(userBlacklist).Append(filter);

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

        public async Task<IEnumerable<string>> GetBlacklistedTags(string nick) => await cache.GetOrCreateAsync(GetCacheEntry(nick), async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(15);

            return await context.Blacklists.AsQueryable()
                .Where(b => b.Nick.Equals(nick, StringComparison.InvariantCultureIgnoreCase))
                .Select(b => b.Tag)
                .ToListAsync();
        });

        public async Task BlacklistTags(string nick, IEnumerable<string> tags)
        {
            var existingTags = await context.Blacklists.AsQueryable()
                .Where(t => t.Nick.Equals(nick, StringComparison.InvariantCultureIgnoreCase))
                .Where(t => tags.Contains(t.Tag))
                .ToListAsync();

            var tagsToAdd = tags
                .Where(t => !existingTags.Any(et => et.Tag == t))
                .Select(t => new BooruBlacklist
                {
                    Nick = nick,
                    Tag = t
                });

            context.Blacklists.AddRange(tagsToAdd);
            await context.SaveChangesAsync();
            ClearCache(nick);
        }

        public async Task WhitelistTags(string nick, IEnumerable<string> tags)
        {
            context.Blacklists.RemoveRange(context.Blacklists.AsQueryable().Where(b => b.Nick.Equals(nick, StringComparison.InvariantCultureIgnoreCase) && tags.Contains(b.Tag)));

            await context.SaveChangesAsync();
            ClearCache(nick);
        }

        private void ClearCache(string nick) => cache.Remove(GetCacheEntry(nick));

        private static string GetCacheEntry(string nick) => $"boorublacklist:{nick}";

        private static IEnumerable<string> Negate(IEnumerable<string> tags) => tags.Select(t => $"-{t}");
    }
}
