using System;
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
    private static string _username;

    public BotKarmaReactHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(KarmaChangeNotification notification, CancellationToken cancellationToken)
    {
        var username = GetUsername();
        if (notification.Subject.Equals(username, StringComparison.OrdinalIgnoreCase))
        {
            if (notification.NewValue > notification.OldValue)
            {
                await notification.TriggeringMessage.RespondAsync("yee");
                await notification.TriggeringMessage.CreateReactionAsync(DiscordEmoji.FromUnicode("😃"));
            }
            else
            {
                await notification.TriggeringMessage.RespondAsync("fak");
                await notification.TriggeringMessage.CreateReactionAsync(DiscordEmoji.FromUnicode("😭"));
            }
        }
    }

    private string GetUsername()
    {
        if (_username is not null)
            return _username;

        using var scope = _serviceScopeFactory.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<DiscordClient>();
        _username = client.CurrentUser.Username;
        return _username;
    }
}