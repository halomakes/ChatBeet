using BooruSharp.Booru;
using BooruSharp.Search.Post;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using MediatR;

namespace ChatBeet.Services;

public class BooruService
{
    private readonly IMemoryCache _cache;
    private readonly Gelbooru _gelbooru;
    private readonly ChatBeetConfiguration.BooruConfiguration _booruConfig;
    private readonly BooruContext _context;
    private readonly IMediator _messageQueue;
    private readonly HttpClient _httpClient;

    public BooruService(IOptions<ChatBeetConfiguration> options, IMemoryCache cache, Gelbooru gelbooru, BooruContext context, IMediator messageQueue, HttpClient httpClient)
    {
        _cache = cache;
        _gelbooru = gelbooru;
        _booruConfig = options.Value.Booru;
        _context = context;
        _messageQueue = messageQueue;
        _httpClient = httpClient;
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
        var filter = safeContentOnly.HasValue ? (safeContentOnly.Value ? "rating:general" : "-rating:general") : string.Empty;
        var globalBlacklist = Negate(_booruConfig.BlacklistedTags);
        var userBlacklist = string.IsNullOrEmpty(requestor)
            ? new List<string>()
            : Negate(await GetBlacklistedTags(requestor));

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

    public IEnumerable<string> GetGlobalBlacklistedTags() => _booruConfig.BlacklistedTags;

    public async Task<IEnumerable<string>> GetBlacklistedTags(string nick) => await _cache.GetOrCreateAsync(GetCacheEntry(nick), entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(15);

        return _context.Blacklists.AsNoTracking()
            .Where(b => b.Nick == nick)
            .Select(b => b.Tag)
            .ToListAsync();
    });

    public async Task BlacklistTags(string nick, IEnumerable<string> tags)
    {
        var existingTags = await _context.Blacklists.AsQueryable()
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

        _context.Blacklists.AddRange(tagsToAdd);
        await _context.SaveChangesAsync();
        ClearCache(nick);
    }

    public async Task WhitelistTags(string nick, IEnumerable<string> tags)
    {
        _context.Blacklists.RemoveRange(_context.Blacklists.AsQueryable().Where(b => b.Nick == nick && tags.Contains(b.Tag)));

        await _context.SaveChangesAsync();
        ClearCache(nick);
    }

    public async Task<IEnumerable<string>> GetTagsAsync(string query) => await _cache.GetOrCreateAsync($"booru:tags:{query}", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromHours(1);
        var response = await _httpClient.GetFromJsonAsync<TagResponse>($"https://gelbooru.com/index.php?page=dapi&s=tag&q=index&json=1&limit=25&name_pattern={query}%");
        return response.Tag.Select(t => t.Name).ToList();
    });

    private void ClearCache(string nick) => _cache.Remove(GetCacheEntry(nick));

    private static string GetCacheEntry(string nick) => $"boorublacklist:{nick}";

    private static IEnumerable<string> Negate(IEnumerable<string> tags) => tags.Select(t => $"-{t}");

    public async Task RecordTags(string nick, IEnumerable<string> tags)
    {
        try
        {
            var usableTags = tags.Where(t => !t.StartsWith("-")).Where(t => !t.Contains(":"));
            var tagEntries = usableTags.Select(t => new TagHistory { Nick = nick, Tag = t });
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