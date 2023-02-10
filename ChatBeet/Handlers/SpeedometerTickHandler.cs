using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using ChatBeet.Services;
using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Handlers;

public class SpeedometerTickHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    public Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        SpeedometerService.RecordMessage(notification.Event.Channel.Id);
        return Task.CompletedTask;
    }
}