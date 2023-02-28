using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Configuration;
using ChatBeet.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class DefUpdatedHandler : INotificationHandler<DefinitionChange>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static DiscordChannel? _channel;

    public DefUpdatedHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(DefinitionChange notification, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var discord = scope.ServiceProvider.GetRequiredService<DiscordClient>();
        var discordConfig = scope.ServiceProvider.GetRequiredService<DiscordBotConfiguration>();
        _channel ??= await discord.GetChannelAsync(discordConfig.Channels["Audit"]);
        await discord.SendMessageAsync(_channel,
            $"{Formatter.Bold(notification.NewUser.Mention())} set {Formatter.Bold(notification.Key)} = {notification.NewValue}");
        if (string.IsNullOrEmpty(notification.OldValue))
            return;
        await discord.SendMessageAsync(_channel,
            $"Previous value was {Formatter.Bold(notification.OldValue)}, set by {notification.OldUser?.Mention()}.");
    }
}