using BooruSharp.Booru;
using BooruSharp.Search.Post;
using ChatBeet.Configuration;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Services;

public class BooruService
{
    private readonly IMemoryCache _cache;
    private readonly Gelbooru _gelbooru;
    private readonly ChatBeetConfiguration.BooruConfiguration _booruConfig;
    private readonly IBooruRepository _context;
    private readonly IMediator _messageQueue;
    private readonly HttpClient _httpClient;

    public BooruService(IOptions<ChatBeetConfiguration> options, IMemoryCache cache, Gelbooru gelbooru, IBooruRepository context, IMediator messageQueue, HttpClient httpClient)
    {
        _cache = cache;
        _gelbooru = gelbooru;
        _booruConfig = options.Value.Booru;
        _context = context;
        _messageQueue = messageQueue;
        _httpClient = httpClient;
    }

    public Task<MediaSearchResult?> GetRandomPostAsync(Rating rating, Guid userId, params string[] tags) => GetRandomPostAsync(rating, userId, tags.AsEnumerable());

    public async Task<MediaSearchResult?> GetRandomPostAsync(Rating rating, Guid userId, IEnumerable<string> tags = null)
    {
        var filter = $"rating:{rating.ToString().ToLower()}";
        var globalBlacklist = Negate(_booruConfig.BlacklistedTags);
        var userBlacklist = Negate(await GetBlacklistedTags(userId));

        var allTags = tags.Concat(globalBlacklist).Concat(userBlacklist).Append(filter);

        var results = await _cache.GetOrCreateAsync($"booru:{string.Join("|", allTags.OrderBy(t => t))}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            return _gelbooru.GetRandomPostsAsync(20, allTags.ToArray());
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
                    .OrderByDescending(p => p.MatchesInput ? 1 : 0)
                    .ThenBy(_ => rng.Next())
                    .Select(p => p.Tag)
                    .Take(10)
                    .ToList();
                return new(img.FileUrl, img.PostUrl, img.Rating, resultTags);
            }

            return default;
        }
    }

    public IEnumerable<string> GetGlobalBlacklistedTags() => _booruConfig.BlacklistedTags;

    public async Task<List<string>> GetBlacklistedTags(Guid userId) => await _cache.GetOrCreateAsync(GetCacheEntry(userId), entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(15);

        return _context?.BlacklistedTags?.AsNoTracking()
            .Where(b => b.UserId == userId)
            .Select(b => b.Tag)
            .ToListAsync();
    });

    public async Task BlacklistTags(Guid userId, IEnumerable<string> tags)
    {
        var existingTags = await _context.BlacklistedTags.AsQueryable()
            .Where(t => t.UserId == userId)
            .Where(t => tags.Contains(t.Tag))
            .ToListAsync();

        var tagsToAdd = tags
            .Where(t => !existingTags.Any(et => et.Tag == t))
            .Select(t => new BlacklistedTag
            {
                UserId = userId,
                Tag = t
            });

        _context.BlacklistedTags.AddRange(tagsToAdd);
        await _context.SaveChangesAsync();
        ClearCache(userId);
    }

    public async Task WhitelistTags(Guid userId, IEnumerable<string> tags)
    {
        _context.BlacklistedTags.RemoveRange(_context.BlacklistedTags.AsQueryable().Where(b => b.UserId == userId && tags.Contains(b.Tag)));

        await _context.SaveChangesAsync();
        ClearCache(userId);
    }

    public async Task<IEnumerable<string>> GetTagsAsync(string query) => await _cache.GetOrCreateAsync($"booru:tags:{query}", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromHours(1);
        var response = await _httpClient.GetFromJsonAsync<TagResponse>($"https://gelbooru.com/index.php?page=dapi&s=tag&q=index&json=1&limit=25&name_pattern={query}%");
        return response.Tag.Select(t => t.Name).ToList();
    });

    private void ClearCache(Guid userId) => _cache.Remove(GetCacheEntry(userId));

    private static string GetCacheEntry(Guid userId) => $"boorublacklist:{userId}";

    private static IEnumerable<string> Negate(IEnumerable<string> tags) => tags.Select(t => $"-{t}");

    public async Task RecordTags(Guid userId, IEnumerable<string> tags)
    {
        try
        {
            var usableTags = tags.Where(t => !t.StartsWith("-")).Where(t => !t.Contains(":"));
            var tagEntries = usableTags.Select(t => new TagHistory { UserId = userId, Tag = t });
            _context.TagHistories.AddRange(tagEntries);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await _messageQueue.Publish(new BackendExceptionNotification(e));
        }
    }

    public record struct MediaSearchResult(Uri ImageUrl, Uri PageUrl, Rating Rating, IEnumerable<string> Tags);

    internal record TagResponseEntry(string Name);

    internal record TagResponse(IEnumerable<TagResponseEntry> Tag);
}