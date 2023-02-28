using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands;
using ChatBeet.Notifications;
using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Handlers;

public class WhipReactHandler : INotificationHandler<DiscordNotification<MessageReactionAddEventArgs>>
{
    public async Task Handle(DiscordNotification<MessageReactionAddEventArgs> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.Message.Author.Id != notification.Client.CurrentUser.Id || notification.Event.User.Id == notification.Client.CurrentUser.Id || notification.Event.Emoji.Name != WhipCommandModule.Emoji)
            return;
        if (!WhipCommandModule.CanUpdate(notification.Event.Message, notification.Event.User))
            return;

        await WhipCommandModule.UpdateComparison(notification.Event.Channel.Id, notification.Event.User);
    }
}