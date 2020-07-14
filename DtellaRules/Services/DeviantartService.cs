using DtellaRules.Utilities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace DtellaRules.Services
{
    public class DeviantartService
    {
        private readonly HttpClient client;
        private readonly IMemoryCache cache;

        public DeviantartService(IHttpClientFactory clientFactory, IMemoryCache cache)
        {
            client = clientFactory.CreateClient();
            this.cache = cache;
        }

        public async Task<string> GetRecentImageAsync(string search)
        {
            var items = await cache.GetOrCreateAsync($"deviantart:{search}", async e =>
            {
                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                var url = $"https://backend.deviantart.com/rss.xml?type=deviation&q={HttpUtility.UrlEncode(search)}";
                var response = await client.GetAsync(url);
                var reader = XmlReader.Create(new StringReader(await response.Content.ReadAsStringAsync()));
                var feed = SyndicationFeed.Load(reader);
                return feed.Items.Select(i => i.Id).ToList();
            });

            return items.PickRandom();
        }
    }
}
