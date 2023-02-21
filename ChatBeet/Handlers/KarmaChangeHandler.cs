using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
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

public partial class KarmaChangeHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public KarmaChangeHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [GeneratedRegex(@"^(\S+)\s?((?:\+\+)|(?:\-\-)|(?:–))$")]
    public partial Regex Rgx();

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        var match = Rgx().Match(notification.Event.Message.Content);
        if (!match.Success)
            return;

        var target = match.Groups[1].Value;
        var mode = match.Groups[2].Value == "++" ? KarmaVote.VoteType.Up : KarmaVote.VoteType.Down;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var karma = scope.ServiceProvider.GetRequiredService<KarmaService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var currentUser = await userService.GetUserAsync(notification.Event.Author);
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        try
        {
            await karma.RecordVote(notification.Event.Guild.Id, target, currentUser, mode);
            var level = await karma.GetLevelAsync(notification.Event.Guild.Id, target);
            await notification.Event.Message.RespondAsync(new DiscordMessageBuilder()
                .WithContent($"{target.ToPossessive()} karma is now {level}."));
            await mediator.Publish(new KarmaChangeNotification(
                    notification.Event.Message,
                    await karma.GetCanonicalKeyAsync(notification.Event.Guild.Id, target),
                    level,
                    level + (mode == KarmaVote.VoteType.Up ? -1 : 1))
                , cancellationToken);
        }
        catch (KarmaRateLimitException e)
        {
            await notification.Event.Message.RespondAsync(new DiscordMessageBuilder()
                .WithContent($"You can change {target.ToPossessive()} karma again {Formatter.Timestamp(e.Delay)}."));
        }
    }
}