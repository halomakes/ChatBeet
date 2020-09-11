using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using GravyIrc.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class KeywordService
    {
        private readonly KeywordContext db;
        private readonly IMemoryCache cache;
        private static bool initialized;

        public static DateTime StatsLastUpdated { get; private set; }

        public KeywordService(KeywordContext db, IMemoryCache cache)
        {
            this.db = db;
            this.cache = cache;
            if (!initialized)
            {
                this.db.Database.EnsureCreated();
                initialized = true;
            }
        }

        public Task<List<Keyword>> GetKeywordsAsync() => cache.GetOrCreateAsync("keywords", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await db.Keywords.AsQueryable().ToListAsync();
        });

        public Task RecordKeywordEntryAsync(Keyword keyword, PrivateMessage message) => RecordKeywordEntryAsync(keyword.Id, message);

        public async Task RecordKeywordEntryAsync(int keywordId, PrivateMessage message)
        {
            var record = new KeywordRecord
            {
                KeywordId = keywordId,
                Message = message.Message,
                Nick = message.From,
                Time = message.DateReceived
            };
            db.Records.Add(record);
            await db.SaveChangesAsync();
        }

        public async Task<Keyword> GetKeywordAsync(string label)
        {
            var keywords = await GetKeywordsAsync();
            return keywords.FirstOrDefault(k => k.Label == label);
        }

        public Task<KeywordStat> GetKeywordStatAsync(string label) => cache.GetOrCreateAsync($"keywords:stats:{label}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var keyword = await GetKeywordAsync(label);
            var stats = await db.Records
                .AsQueryable()
                .Where(r => r.KeywordId == keyword.Id)
                .GroupBy(r => r.Nick)
                .OrderByDescending(g => g.Count())
                .Select(g => new KeywordStat.UserKeywordStat
                {
                    Nick = g.Key,
                    Hits = g.Count(),
                    Excerpt = g.Min(gp => gp.Message)
                })
                .ToListAsync();

            return new KeywordStat
            {
                Keyword = keyword,
                Stats = stats
            };
        });

        public Task<IEnumerable<KeywordStat>> GetKeywordStatsAsync() => cache.GetOrCreateAsync("keyword:stats", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            StatsLastUpdated = DateTime.UtcNow;

            var keywords = await GetKeywordsAsync();
            var allStats = await db.Records
                .AsQueryable()
                .GroupBy(r => new { r.KeywordId, r.Nick })
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    g.Key.KeywordId,
                    Stats = new KeywordStat.UserKeywordStat
                    {
                        Nick = g.Key.Nick,
                        Hits = g.Count(),
                        Excerpt = g.Min(gp => gp.Message)
                    }
                })
                .ToListAsync();
            return keywords.Select(keyword => new KeywordStat
            {
                Keyword = keyword,
                Stats = allStats
                    .Where(s => s.KeywordId == keyword.Id)
                    .Select(s => s.Stats)
            });
        });
    }
}
