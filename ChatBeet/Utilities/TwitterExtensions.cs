using GravyBot;
using Humanizer;
using LinqToTwitter;

namespace ChatBeet.Utilities
{
    public static class TwitterExtensions
    {
        public static string ToIrcMessage(this Status tweet) => $"{IrcValues.BOLD}{tweet.User?.Name}{IrcValues.RESET} {tweet.CreatedAt.Humanize()} - {tweet.Text?.RemoveLineBreaks(" | ")}";
    }
}
