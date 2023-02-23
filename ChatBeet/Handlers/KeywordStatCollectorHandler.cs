using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Notifications;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class KeywordStatCollectorHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public const string MiataKeywordType = "keyword:miata";
    public const string BruhKeywordType = "keyword:bruh";
    public const string RipKeywordType = "keyword:rip";
    public const string YeetKeywordType = "keyword:yeet";

    private const int SurroundingChars = 20;

    private static readonly Dictionary<string, string> Keywords = new()
    {
        { MiataKeywordType, "miata" },
        { BruhKeywordType, "bruh" },
        { RipKeywordType, "rip" },
        { YeetKeywordType, "yeet" }
    };

    public KeywordStatCollectorHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        var content = notification.Event.Message.Content;
        var matches = Keywords
            .Select(k => (Keyword: k, Index: content.IndexOf(k.Value, StringComparison.InvariantCultureIgnoreCase)))
            .Where(t => t.Index >= 0)
            .ToList();
        if (!matches.Any())
            return;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var usersRepo = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var statsRepo = scope.ServiceProvider.GetRequiredService<IStatsRepository>();
        var user = await usersRepo.GetUserAsync(notification.Event.Author, cancellationToken);

        var entries = matches.Select(m =>
        {
            var (startIndex, isStartTruncated) = m.Index > SurroundingChars
                ? (m.Index - SurroundingChars, true)
                : (0, false);
            var (size, isEndTruncated) = SurroundingChars * 2 + startIndex >= content.Length
                ? (content.Length - startIndex, false)
                : (SurroundingChars * 2, true);
            var sampleText = $"{(isStartTruncated ? "…" : "")}{content.Substring(startIndex, size)}{(isEndTruncated ? "…" : "")}";
            return new StatEvent
            {
                GuildId = notification.Event.Guild.Id,
                SampleText = sampleText,
                EventType = m.Keyword.Key,
                OccurredAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                TriggeringUserId = user.Id
            };
        });
        statsRepo.StatEvents.AddRange(entries);
        await statsRepo.SaveChangesAsync(cancellationToken);
    }
}