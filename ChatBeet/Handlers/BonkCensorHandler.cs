using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using ChatBeet.Utilities;
using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Handlers;

public class BonkCensorHandler : INotificationHandler<BonkableMessageNotification>, INotificationHandler<DiscordNotification<MessageReactionAddEventArgs>>
{
    private static readonly SlidingBuffer<ulong> EligibleMessageIds = new(20);
    
    public Task Handle(BonkableMessageNotification notification, CancellationToken cancellationToken)
    {
        EligibleMessageIds.Add(notification.Message.Id);
        return Task.CompletedTask;
    }

    public async Task Handle(DiscordNotification<MessageReactionAddEventArgs> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.Message.Author.Id != notification.Client.CurrentUser.Id || notification.Event.Emoji.Name != "bonk")
            return;
        if (EligibleMessageIds.Contains(notification.Event.Message.Id))
            await notification.Event.Message.ModifyEmbedSuppressionAsync(true);
    }
}