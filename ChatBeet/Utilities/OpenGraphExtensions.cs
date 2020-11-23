using GravyBot;
using HtmlAgilityPack;
using OpenGraphNet;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBeet.Utilities
{
    public static class OpenGraphExtensions
    {
        public static string ToIrcSummary(this OpenGraph meta, int maxDescriptionLength = 50)
        {
            HtmlDocument htmlDoc = default;

            HtmlDocument GetHtmlDoc()
            {
                if (htmlDoc == default)
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(meta.OriginalHtml);
                }
                return htmlDoc;
            }

            string GetTitle()
            {
                if (!string.IsNullOrEmpty(meta.Title))
                {
                    return meta.Title;
                }
                else
                {
                    // try to get title from title tag since opengraph is missing
                    try
                    {
                        var title = GetHtmlDoc().DocumentNode.Descendants("title").FirstOrDefault();
                        if (title != default && !string.IsNullOrEmpty(title.InnerText))
                            return title.InnerText;
                    }
                    catch
                    {
                        return default;
                    }
                }
                return default;
            }

            string GetDescription()
            {
                // get from opengraph
                if (meta.Metadata.ContainsKey("og:description"))
                {
                    var ogDescription = HttpUtility.HtmlDecode(meta.Metadata["og:description"].Select(m => m.Value?.Trim()).FirstOrDefault(v => !string.IsNullOrEmpty(v)));
                    if (ogDescription != default)
                        return ogDescription;
                }

                // look for meta tag
                try
                {
                    var node = GetHtmlDoc().DocumentNode.SelectSingleNode("//meta[@name='description']");

                    if (node != null)
                    {
                        var desc = node.Attributes["content"];
                        if (!string.IsNullOrEmpty(desc?.Value))
                            return desc.Value;
                    }
                }
                catch
                {
                    return default;
                }

                return default;
            }

            IEnumerable<string> GetSegments()
            {
                var title = GetTitle();
                if (!string.IsNullOrEmpty(title))
                    yield return $"{IrcValues.BOLD}{title}{IrcValues.RESET}";

                var description = GetDescription();
                if (!string.IsNullOrEmpty(description))
                {
                    if (description.Length > maxDescriptionLength)
                        yield return $"{description.Substring(0, maxDescriptionLength).Trim()}…";
                    else
                        yield return description;
                }
            }
            return string.Join(" | ", GetSegments());
        }
    }
}
