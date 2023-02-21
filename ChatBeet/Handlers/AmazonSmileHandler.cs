using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using DSharpPlus.EventArgs;
using MediatR;
using System.Text.RegularExpressions;

namespace ChatBeet.Handlers;

public partial class AmazonSmileHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification,
        CancellationToken cancellationToken)
    {
        var match = Rgx().Match(notification.Event.Message.Content);
        if (match.Success)
        {
            var url = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(url))
            {
                var modified = ModifyUri(url);
                if (!string.IsNullOrEmpty(modified))
                    await notification.Event.Message.RespondAsync(modified);
            }
        }
    }

    [GeneratedRegex(@"((?:https?:\/\/)?(?:www.amazon\.com)\/\S+)")]
    private static partial Regex Rgx();

    /// <summary>
    /// Changes domain to smile.amazon.com and strips any tracking info from query
    /// </summary>
    private static string ModifyUri(string original)
    {
        try
        {
            var originalBuilder = new UriBuilder(original);
            return new UriBuilder
            {
                Host = "smile.amazon.com",
                Scheme = "https",
                Path = originalBuilder.Path
            }.ToString();
        }
        catch
        {
            return default;
        }
    }
}