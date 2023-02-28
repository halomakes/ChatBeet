using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Notifications;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public partial class SuspectHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SuspectHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    [GeneratedRegex(@"^\<@\d+\> sus$")]
    private partial Regex Rgx();

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        if (!Rgx().IsMatch(notification.Event.Message.Content))
            return;
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var discord = scope.ServiceProvider.GetRequiredService<DiscordClient>();
        var negativeResponseService = scope.ServiceProvider.GetRequiredService<NegativeResponseService>();
        var service = scope.ServiceProvider.GetRequiredService<SuspicionService>();
        var usersRepo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        
        var suspect = notification.Event.MentionedUsers.Single();
        if (suspect.Equals(discord.CurrentUser))
        {
            await negativeResponseService.Respond(notification.Event.Message);
        }
        else
        {
            var internalSuspect = await usersRepo.GetUserAsync(suspect, cancellationToken);
            var internalReporter = await usersRepo.GetUserAsync(notification.Event.Author, cancellationToken);
            if (await service.HasRecentlyReportedAsync(notification.Event.Guild.Id, internalSuspect.Id, internalReporter.Id))
            {
                await notification.Event.Message.RespondAsync("You must wait at least 2 minutes each time you raise suspicion against a user.");
            }
            else
            {
                await service.ReportSuspiciousActivityAsync(notification.Event.Guild.Id, internalSuspect.Id, internalReporter.Id, bypassDebounceCheck: true);

                var suspicionLevel = await service.GetSuspicionLevelAsync(notification.Event.Guild.Id, internalSuspect.Id);

                await notification.Event.Message.RespondAsync($"{Formatter.Mention(suspect)}{suspect.Username.GetPossessiveSuffix()} suspicion level is now {suspicionLevel}.");
            }
        }
        await notification.Event.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🕵"));
    }
}