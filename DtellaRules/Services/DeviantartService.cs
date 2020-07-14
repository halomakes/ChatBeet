using DtellaRules.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace DtellaRules.Services
{
    public class DeviantartService
    {
        private readonly HttpClient client;

        public DeviantartService(IHttpClientFactory clientFactory)
        {
            client = clientFactory.CreateClient();
        }

        public async Task<string> GetRecentImageAsync(string search)
        {
            var url = $"https://backend.deviantart.com/rss.xml?type=deviation&q={HttpUtility.UrlEncode(search)}";
            var response = await client.GetAsync(url);
            var reader = XmlReader.Create(new StringReader(await response.Content.ReadAsStringAsync()));
            var feed = SyndicationFeed.Load(reader);

            var imageUrl = feed.Items
                .PickRandom()
                .ElementExtensions
                .Where(i => i.OuterName == "thumbnail")
                .Select(i => i.GetObject<XElement>())
                .OrderByDescending(i => int.TryParse(i.Attribute("height")?.Value, out var num) ? num : 0)
                .Select(i => i.Attribute("url").Value)
                .FirstOrDefault();

            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                return uri.GetLeftPart(UriPartial.Path);

            return null;
        }
    }
}
