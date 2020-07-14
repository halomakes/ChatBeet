using ChatBeet;
using Miki.Anilist;
using System.Text.RegularExpressions;

namespace DtellaRules.Utilities
{
    public static class AnilistExtensions
    {
        public static string GetSimplifiedDescription(this ICharacter @char)
        {
            if (!string.IsNullOrEmpty(@char.Description))
            {
                var singleLine = @char.Description.Replace("\n", " • ");
                var labelRegex = new Regex(@"__([A-z ]*):__");
                return labelRegex.Replace(singleLine, $"{IrcValues.BOLD}$1{IrcValues.RESET}: ");
            }
            return null;
        }
    }
}
