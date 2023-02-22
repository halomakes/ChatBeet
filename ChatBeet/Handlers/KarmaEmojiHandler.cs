using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Exceptions;
using ChatBeet.Notifications;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class KarmaEmojiHandler : INotificationHandler<DiscordNotification<MessageReactionAddEventArgs>>
{
    private static readonly DiscordEmoji UpEmoji = DiscordEmoji.FromUnicode("📈");
    private static readonly DiscordEmoji DownEmoji = DiscordEmoji.FromUnicode("📉");

    private readonly IServiceScopeFactory _scopeFactory;

    public KarmaEmojiHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(DiscordNotification<MessageReactionAddEventArgs> notification, CancellationToken cancellationToken)
    {
        if (!(notification.Event.Emoji.Name == UpEmoji.Name || notification.Event.Emoji.Name == DownEmoji.Name))
            return;
        await using var scope = _scopeFactory.CreateAsyncScope();
        var karma = scope.ServiceProvider.GetRequiredService<KarmaService>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        try
        {
            var user = await userRepo.GetUserAsync(notification.Event.User);
            var mention = Formatter.Mention(notification.Event.Message.Author);
            if (notification.Event.Emoji.Name == UpEmoji.Name)
                await karma.IncrementAsync(notification.Event.Guild.Id, mention, user);
            else
                await karma.DecrementAsync(notification.Event.Guild.Id, mention, user);
            var level = await karma.GetLevelAsync(notification.Event.Guild.Id, mention);
            await notification.Event.Message.RespondAsync($"{mention.ToPossessive()} karma is now {level}.");
        }
        catch (SelfKarmaException)
        {
        }
        catch (KarmaRateLimitException)
        {
        }
    }
}