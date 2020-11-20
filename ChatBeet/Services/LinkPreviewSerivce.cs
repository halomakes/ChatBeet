using Microsoft.Extensions.Caching.Memory;
using OpenGraphNet;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class LinkPreviewSerivce
    {
        private readonly IMemoryCache cache;

        public LinkPreviewSerivce(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public Task<OpenGraph> GetMetadataAsync(string url) => cache.GetOrCreateAsync($"opengraph:{url}", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            return OpenGraph.ParseUrlAsync(url);
        });

        public Task<OpenGraph> GetMetadataAsync(Uri url) => cache.GetOrCreateAsync($"opengraph:{url}", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            return OpenGraph.ParseUrlAsync(url);
        });
    }
}
