using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Notifications;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class GuildInfoHandler : INotificationHandler<DiscordNotification<GuildCreateEventArgs>>, INotificationHandler<DiscordNotification<GuildUpdateEventArgs>>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public GuildInfoHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(DiscordNotification<GuildCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        await StoreGuild(notification.Event.Guild, cancellationToken);
    }

    public async Task Handle(DiscordNotification<GuildUpdateEventArgs> notification, CancellationToken cancellationToken)
    {
        await StoreGuild(notification.Event.GuildAfter, cancellationToken);
    }

    private async Task StoreGuild(DiscordGuild guild, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var existingGuild = await context.Guilds.FindAsync(guild.Id, cancellationToken);
        if (existingGuild is null)
        {
            var user = await context.GetUserAsync(guild.Owner);
            context.Guilds.Add(new Guild
            {
                Id = guild.Id,
                Label = guild.Name,
                AddedBy = user.Id,
                AddedAt = guild.JoinedAt.UtcDateTime
            });
            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            existingGuild.Label = guild.Name;
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}