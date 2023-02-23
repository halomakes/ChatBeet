using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class BotKarmaReactHandler : INotificationHandler<KarmaChangeNotification>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static string? _mention;

    public BotKarmaReactHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(KarmaChangeNotification notification, CancellationToken cancellationToken)
    {
        if (notification.TriggeringMessage is null)
            return;
        var username = GetUsername();
        if (notification.Subject.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            if (notification.NewValue > notification.OldValue)
                await notification.TriggeringMessage.CreateReactionAsync(DiscordEmoji.FromUnicode("😃"));
            else
                await notification.TriggeringMessage.CreateReactionAsync(DiscordEmoji.FromUnicode("😭"));
        }
    }

    private string GetUsername()
    {
        if (_mention is not null)
            return _mention;

        using var scope = _serviceScopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<DiscordClient>();
        _mention = client.CurrentUser.Mention;
        return _mention;
    }
}