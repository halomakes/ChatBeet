using GravyBot;
using Humanizer;
using LinqToTwitter;
using System.Web;

namespace ChatBeet.Utilities;

public static class TwitterExtensions
{
    public static string ToIrcMessage(this Status tweet) => $"{IrcValues.BOLD}{tweet.User?.Name}{IrcValues.RESET} {tweet.CreatedAt.Humanize()} - {HttpUtility.HtmlDecode(tweet.FullText ?? tweet.Text).RemoveLineBreaks(" | ")}";
}