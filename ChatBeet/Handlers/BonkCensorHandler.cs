using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class BonkCensorHandler : INotificationHandler<BonkableMessageNotification>, INotificationHandler<DiscordNotification<MessageReactionAddEventArgs>>
{
    private static readonly SlidingBuffer<ulong> EligibleMessageIds = new(20);
    private static ulong _userId;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BonkCensorHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task Handle(BonkableMessageNotification notification, CancellationToken cancellationToken)
    {
        EligibleMessageIds.Add(notification.Message.Id);
        return Task.CompletedTask;
    }

    public async Task Handle(DiscordNotification<MessageReactionAddEventArgs> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.Message.Author.Id != GetUserId() || notification.Event.Emoji.Name != "bonk")
            return;
        if (EligibleMessageIds.Contains(notification.Event.Message.Id))
            await notification.Event.Message.ModifyEmbedSuppressionAsync(true);
    }

    private ulong GetUserId()
    {
        if (_userId != default)
            return _userId;

        using var scope = _serviceScopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<DiscordClient>();
        _userId = client.CurrentUser.Id;
        return _userId;
    }
}