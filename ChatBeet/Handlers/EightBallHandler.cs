using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using ChatBeet.Utilities;
using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Handlers;

public partial class EightBallHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    [GeneratedRegex(@"^\<@\d+\>\W+\w+.*\?$", RegexOptions.IgnoreCase)]
    private static partial Regex Rgx();

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        if (Rgx().IsMatch(notification.Event.Message.Content)
            && notification.Event.MentionedUsers.FirstOrDefault()!.Id == notification.Client.CurrentUser.Id)
        {
            await notification.Event.Message.RespondAsync(YesNoGenerator.GetResponse());
        }
    }
}