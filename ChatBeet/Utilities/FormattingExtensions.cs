using DSharpPlus;
using GravyBot;
using System;

namespace ChatBeet.Utilities
{
    public static class FormattingExtensions
    {
        public static string DiscordColorize(this string text, int? rating) => Formatter.Colorize(text, GetDiscordColor(rating));

        public static string Colorize(this string text, double? rating) => text.Colorize(rating.HasValue ? Convert.ToInt32(rating) : (int?)null);

        public static string Colorize(this string text, int? rating) => $"{GetColor(rating)}{text}{IrcValues.RESET}";

        private static string GetColor(int? rating)
        {
            if (!rating.HasValue)
                return IrcValues.RESET;
            if (rating > 90)
                return IrcValues.GREEN;
            else if (rating > 80)
                return IrcValues.LIME;
            else if (rating > 70)
                return IrcValues.YELLOW;
            else if (rating > 60)
                return IrcValues.ORANGE;
            else if (rating > 50)
                return IrcValues.RED;
            else if (rating > 30)
                return IrcValues.BROWN;
            return IrcValues.RESET;
        }

        private static AnsiColor GetDiscordColor(int? rating) => rating switch
        {
            null => AnsiColor.Reset,
            > 80 => AnsiColor.Green,
            > 60 => AnsiColor.Yellow,
            > 40 => AnsiColor.Red,
            _ => AnsiColor.Magenta
        };
    }
}
