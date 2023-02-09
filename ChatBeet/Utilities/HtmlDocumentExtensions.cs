using GravyBot;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ChatBeet.Utilities;

public static class HtmlDocumentExtensions
{
    public static string ToIrcSummary(this HtmlDocument doc, int maxDescriptionLength = 50)
    {
        string GetTagContent(string xpath, string attribute)
        {
            try
            {
                var node = doc.DocumentNode.SelectSingleNode(xpath);

                if (node is not null)
                {
                    var desc = node.Attributes[attribute];
                    if (!string.IsNullOrEmpty(desc?.Value))
                        return WebUtility.HtmlDecode(desc.Value);
                }
                return default;
            }
            catch
            {
                return default;
            }
        }

        string GetTitle()
        {
            var ogTitle = GetTagContent(@"//meta[@property='og:title']", "content");
            if (!string.IsNullOrEmpty(ogTitle))
                return ogTitle;

            var docTitle = doc.DocumentNode.Descendants("title").FirstOrDefault();
            if (docTitle != default && !string.IsNullOrEmpty(docTitle.InnerText))
                return WebUtility.HtmlDecode(docTitle.InnerText);

            return default;
        }

        string GetDescription() => GetTagContent(@"//meta[@property='og:description']", "content")
                                   ?? GetTagContent(@"//meta[@name='description']", "content");

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