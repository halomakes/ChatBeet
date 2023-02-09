using System.Text.RegularExpressions;

namespace ChatBeet.Utilities;

public static partial class MarkdownExtensions
{
    [GeneratedRegex(@"~!.*!~")]
    private static partial Regex SpoilerRgx();

    public static string RemoveSpoilers(this string @string) => SpoilerRgx().Replace(@string, string.Empty).Trim();
}