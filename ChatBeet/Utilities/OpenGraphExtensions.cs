using GravyBot;
using OpenGraphNet;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Utilities
{
    public static class OpenGraphExtensions
    {
        public static string ToIrcSummary(this OpenGraph meta, int maxDescriptionLength = 50)
        {
            IEnumerable<string> GetSegments()
            {
                if (meta.Metadata.ContainsKey("Title"))
                {
                    var title = meta.Metadata["Title"].Select(m => m.Value?.Trim()).FirstOrDefault(v => !string.IsNullOrEmpty(v));
                    if (title != default)
                        yield return $"{IrcValues.BOLD}{title}{IrcValues.RESET}";
                }
                if (meta.Metadata.ContainsKey("Description"))
                {
                    var description = meta.Metadata["Description"].Select(m => m.Value?.Trim()).FirstOrDefault(v => !string.IsNullOrEmpty(v));
                    if (description != default)
                    {
                        if (description.Length > maxDescriptionLength)
                            yield return $"{description.Substring(0, maxDescriptionLength)}…";
                        else
                            yield return description;
                    }
                }
            }
            return string.Join(" | ", GetSegments());
        }
    }
}
