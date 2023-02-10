using ChatBeet.Configuration;
using ChatBeet.Utilities;
using LinqToTwitter;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class TwitterService
{
    private readonly ChatBeetConfiguration.TwitterConfiguration _twitterConfig;
    private readonly IMemoryCache _cache;

    public TwitterService(IOptions<ChatBeetConfiguration> twitterOptions, IMemoryCache cache)
    {
        _twitterConfig = twitterOptions.Value.Twitter;
        _cache = cache;
    }

    /// <summary>
    /// Get a random recent tweet with an image from an account on Twitter
    /// </summary>
    /// <param name="handle">Handle of username to look up</param>
    /// <param name="mediaOnly">Indicates if results should filter to only tweets with media attached</param>
    /// <param name="randomize">Whether or not to pick a random result from the set</param>
    /// <param name="filter">Filter to apply prior to randomization</param>
    /// <returns>Recent tweet with an image attached</returns>
    public async Task<Status> GetRecentTweet(string handle, bool mediaOnly = true, bool randomize = true, Func<Status, bool> filter = null)
    {
        var tweets = await _cache.GetOrCreateAsync($"twitter:{handle}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            var twitterContext = await GetContext();

            return await twitterContext.Status
                .Where(s => s.Type == StatusType.User)
                .Where(s => s.ScreenName == handle)
                .Where(s => s.RetweetedStatus.StatusID == 0)
                .Where(s => s.InReplyToStatusID == 0)
                .Where(s => s.TweetMode == TweetMode.Extended)
                .Take(30)
                .ToListAsync();
        });

        var filtered = tweets
            .Where(s => !mediaOnly || s.Entities.MediaEntities.Any())
            .Where(s => filter == null || filter(s));

        return randomize
            ? filtered.PickRandom()
            : filtered.FirstOrDefault();
    }

    public async Task<Status> GetRandomTweetByHashtag(string hashtag, bool mediaOnly = true, Func<Status, bool> filter = null)
    {
        var searches = await _cache.GetOrCreateAsync($"twitter:#{hashtag}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
            var twitterContext = await GetContext();

            return await twitterContext.Search
                .Where(s => s.Type == SearchType.Search)
                .Where(s => s.IncludeEntities == true)
                .Where(s => s.TweetMode == TweetMode.Extended)
                .Where(s => s.Query == hashtag)
                .Where(s => s.Count == 100)
                .ToListAsync();
        });

        var filtered = searches.SelectMany(s => s.Statuses)
            .Where(s => !mediaOnly || s.Entities.MediaEntities.Any())
            .Where(s => filter == null || filter(s));

        return filtered.PickRandom();
    }

    /// <summary>
    /// Get a tweet by its ID
    /// </summary>
    /// <param name="id">ID of the tweet</param>
    public async Task<Status> GetTweet(ulong id) => await _cache.GetOrCreateAsync($"twitter:status:{id}", async entry =>
    {
        entry.SlidingExpiration = TimeSpan.FromMinutes(5);
        var twitterContext = await GetContext();

        return await twitterContext.Status
            .Where(s => s.Type == StatusType.Show)
            .Where(s => s.StatusID == id || s.ID == id)
            .Where(s => s.TweetMode == TweetMode.Extended)
            .FirstOrDefaultAsync();
    });

    private async Task<TwitterContext> GetContext()
    {
        var auth = new ApplicationOnlyAuthorizer
        {
            CredentialStore = new InMemoryCredentialStore()
            {
                ConsumerKey = _twitterConfig.ConsumerKey,
                ConsumerSecret = _twitterConfig.ConsumerSecret
            }
        };

        await auth.AuthorizeAsync();
        return new TwitterContext(auth);
    }
}