using DSharpPlus;

namespace ChatBeet.Utilities;

public static class FormattingExtensions
{
    public static string DiscordColorize(this string text, int? rating) => Formatter.Colorize(text, GetDiscordColor(rating));

    private static AnsiColor GetDiscordColor(int? rating) => rating switch
    {
        null => AnsiColor.Reset,
        > 80 => AnsiColor.Green,
        > 60 => AnsiColor.Yellow,
        > 40 => AnsiColor.Red,
        _ => AnsiColor.Magenta
    };
}