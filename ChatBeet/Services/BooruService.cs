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
        private readonly MessageQueueService messageQueue;

        public BooruService(IOptions<ChatBeetConfiguration> options, IMemoryCache cache, Gelbooru gelbooru, BooruContext context, MessageQueueService messageQueue)
        {
            this.cache = cache;
            this.gelbooru = gelbooru;
            booruConfig = options.Value.Booru;
            this.context = context;
            this.messageQueue = messageQueue;
        }

        [Obsolete("Remove with IRC")]
        public Task<string> GetRandomPostFormattedAsync(bool? safeContentOnly, string requestor, params string[] tags) => GetRandomPostFormattedAsync(safeContentOnly, requestor, tags.AsEnumerable());

        [Obsolete("Remove with IRC")]
        public async Task<string> GetRandomPostFormattedAsync(bool? safeContentOnly, string requestor, IEnumerable<string> tags = null)
        {
            var result = await GetRandomPostAsync(safeContentOnly, requestor, tags);
            if (result is MediaSearchResult media)
                return $"{media.ImageUrl} ({media.Rating}) - {string.Join(", ", media.Tags)}";
            return default;
        }

        public Task<MediaSearchResult?> GetRandomPostAsync(bool? safeContentOnly, string requestor, params string[] tags) => GetRandomPostAsync(safeContentOnly, requestor, tags.AsEnumerable());

        public async Task<MediaSearchResult?> GetRandomPostAsync(bool? safeContentOnly, string requestor, IEnumerable<string> tags = null)
        {
            var filter = safeContentOnly.HasValue ? (safeContentOnly.Value ? "rating:sensitive" : "-rating:sensitive") : string.Empty;
            var globalBlacklist = Negate(booruConfig.BlacklistedTags);
            var userBlacklist = string.IsNullOrEmpty(requestor)
                ? new List<string>()
                : Negate(await GetBlacklistedTags(requestor));

            var allTags = tags.Concat(globalBlacklist).Concat(userBlacklist).Append(filter);

            var results = await cache.GetOrCreateAsync($"booru:{string.Join("|", allTags.OrderBy(t => t))}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                return gelbooru.GetRandomPostsAsync(20, allTags.ToArray());
            });

            return PickImage(results);

            MediaSearchResult? PickImage(IEnumerable<SearchResult> searchResults)
            {
                if (searchResults?.Any() ?? false)
                {
                    var img = searchResults.PickRandom();
                    var rng = new Random();
                    var resultTags = img.Tags
                        .Select(t => (MatchesInput: tags.Contains(t), Tag: t))
                        .OrderByDescending(p => p.MatchesInput)
                        .ThenBy(p => rng.Next())
                        .Select(p => p.Tag)
                        .Take(10)
                        .OrderBy(t => rng.Next());
                    return new(img.FileUrl, img.PostUrl, img.Rating, resultTags);
                }
                return default;
            }
        }

        public IEnumerable<string> GetGlobalBlacklistedTags() => booruConfig.BlacklistedTags;

        public async Task<IEnumerable<string>> GetBlacklistedTags(string nick) => await cache.GetOrCreateAsync(GetCacheEntry(nick), entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(15);

            return context.Blacklists.AsNoTracking()
                .Where(b => b.Nick == nick)
                .Select(b => b.Tag)
                .ToListAsync();
        });

        public async Task BlacklistTags(string nick, IEnumerable<string> tags)
        {
            var existingTags = await context.Blacklists.AsQueryable()
                .Where(t => t.Nick == nick)
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
            context.Blacklists.RemoveRange(context.Blacklists.AsQueryable().Where(b => b.Nick == nick && tags.Contains(b.Tag)));

            await context.SaveChangesAsync();
            ClearCache(nick);
        }

        private void ClearCache(string nick) => cache.Remove(GetCacheEntry(nick));

        private static string GetCacheEntry(string nick) => $"boorublacklist:{nick}";

        private static IEnumerable<string> Negate(IEnumerable<string> tags) => tags.Select(t => $"-{t}");

        public async Task RecordTags(string nick, IEnumerable<string> tags)
        {
            try
            {
                var usableTags = tags.Where(t => !t.StartsWith("-")).Where(t => !t.Contains(":"));
                var tagEntries = usableTags.Select(t => new TagHistory { Nick = nick, Tag = t });
                context.TagHistories.AddRange(tagEntries);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                messageQueue.Push(e);
            }
        }

        public record struct MediaSearchResult(Uri ImageUrl, Uri PageUrl, Rating Rating, IEnumerable<string> Tags);
    }
}
