using Markdig;
using Miki.Anilist;

namespace DtellaRules.Utilities
{
    public static class AnilistExtensions
    {
        public static string GetSimplifiedDescription(this ICharacter @char)
        {
            if (!string.IsNullOrEmpty(@char.Description))
            {
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .UseEmphasisExtras()
                    .Build();

                return Markdown.ToPlainText(@char.Description, pipeline);
            }
            return null;
        }
    }
}
