using ChatBeet;
using Markdig;
using System.Linq;
using System.Text;

namespace DtellaRules.Utilities
{
    public static class StringExtensions
    {
        public static bool ExceedsMaxLength(this string message) => message != null && Encoding.Unicode.GetByteCount(message) > IrcValues.MAX_BYTES;

        public static string TruncateMessage(this string message) => $"{message.TruncateToByteLimit(IrcValues.MAX_BYTES - 2)}…";

        public static string TruncateToByteLimit(this string @string, int maxLength) =>
            new string(@string.TakeWhile((c, i) => Encoding.UTF8.GetByteCount(@string.Substring(0, i + 1)) <= maxLength).ToArray());

        public static string StripMarkdown(this string @string)
        {
            if (!string.IsNullOrEmpty(@string))
            {
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UseEmphasisExtras()
                    .Build();

                return Markdown.ToPlainText(@string, pipeline).Trim().Replace("\n", " • ");
            }
            return null;
        }
    }
}
