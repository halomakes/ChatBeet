using GravyBot;
using Markdig;
using System;
using System.Linq;
using System.Text;

namespace ChatBeet.Utilities
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

        public static string RemoveLineBreaks(this string @string, string delimiter = " ") =>
            string.Join(delimiter, @string.Replace("\r\n", "\n")
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
            );

        public static string CapitalizeFirst(this string input) => input switch
        {
            null => null,
            "" => input,
            _ => input.First().ToString().ToUpper() + input.Substring(1).ToLower()
        };

        public static string RemoveLastCharacter(this string @string, char punctuationMark) => @string.EndsWith(punctuationMark) ? @string.RemoveLastCharacter() : @string;

        public static string RemoveLastCharacter(this string @string) => @string.Remove(@string.Length - 1, 1);
    }
}
