using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public partial class EightBallHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static ulong _userId;

    public EightBallHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    [GeneratedRegex(@"^\<@\d+\>\W+\w+.*\?$", RegexOptions.IgnoreCase)]
    private static partial Regex Rgx();

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        if (Rgx().IsMatch(notification.Event.Message.Content)
            && notification.Event.MentionedUsers.FirstOrDefault()!.Id == GetUserId())
        {
            await notification.Event.Message.RespondAsync(YesNoGenerator.GetResponse());
        }
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