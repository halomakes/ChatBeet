using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands.Discord;
using ChatBeet.Notifications;
using ChatBeet.Services;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class IrcLinkValidationHandler : INotificationHandler<DiscordNotification<ModalSubmitEventArgs>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public IrcLinkValidationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(DiscordNotification<ModalSubmitEventArgs> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.Interaction.Data.CustomId != IrcCommandModule.VerifyModalId)
            return;
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var migration = scope.ServiceProvider.GetRequiredService<IrcMigrationService>();
        await migration.ValidateTokenAsync(notification.Event.Interaction, notification.Event.Values["nick"], notification.Event.Values["token"]);
    }
}