using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands;
using ChatBeet.Notifications;
using DSharpPlus;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class WhipReactHandler : INotificationHandler<DiscordNotification<MessageReactionAddEventArgs>>
{
    private static ulong _userId;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WhipReactHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(DiscordNotification<MessageReactionAddEventArgs> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.Message.Author.Id != GetUserId() || notification.Event.User.Id == GetUserId() || notification.Event.Emoji.Name != WhipCommandModule.Emoji)
            return;
        if (!WhipCommandModule.CanUpdate(notification.Event.Message, notification.Event.User))
            return;

        await WhipCommandModule.UpdateComparison(notification.Event.Channel.Id, notification.Event.User);
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