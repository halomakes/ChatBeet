using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Notifications;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public partial class DefinitionLookupHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>, INotificationHandler<DiscordNotification<MessageReactionAddEventArgs>>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private static DiscordEmoji? _reaction;
    private static readonly SlidingBuffer<PreparedResponse> PreparedResponses = new(25);

    public DefinitionLookupHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    [GeneratedRegex(@"(?:^(.+)\?$)|(?:^\?\? (.+)$)")]
    private partial Regex Rgx();

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification, CancellationToken cancellationToken)
    {
        var match = Rgx().Match(notification.Event.Message.Content);
        if (!match.Success)
            return;
        if (match.Groups[1].Success)
            await SendDefinitionHint(notification.Event, match.Groups[1].Value);
        else if (match.Groups[2].Success)
            await SendExplicitResponse(notification.Event, match.Groups[2].Value);
    }

    private async Task SendExplicitResponse(MessageCreateEventArgs @event, string term)
    {
        var def = await GetDefinition(term, @event.Guild.Id);
        if (string.IsNullOrEmpty(def))
            return;
        await @event.Message.RespondAsync($"{Formatter.Bold(term)}: {def}");
    }

    private async Task SendDefinitionHint(MessageCreateEventArgs @event, string term)
    {
        var def = await GetDefinition(term, @event.Guild.Id);
        if (string.IsNullOrEmpty(def))
            return;
        _reaction ??= DiscordEmoji.FromUnicode("📖");
        await @event.Message.CreateReactionAsync(_reaction);
        PreparedResponses.Add(new()
        {
            MessageId = @event.Message.Id,
            Key = term,
            Definition = def
        });
    }

    private async Task<string?> GetDefinition(string term, ulong guildId)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repo = scope.ServiceProvider.GetRequiredService<IDefinitionsRepository>();
        return await repo.Definitions
            .Where(d => d.GuildId == guildId && d.Key.ToLower() == term.ToLower())
            .Select(d => d.Value)
            .FirstOrDefaultAsync();
    }

    private class PreparedResponse
    {
        public ulong MessageId { get; init; }
        public required string Key { get; init; }
        public required string Definition { get; init; }
        public bool Used { get; set; }
    }

    public async Task Handle(DiscordNotification<MessageReactionAddEventArgs> notification, CancellationToken cancellationToken)
    {
        if (_reaction is null)
            return;
        if (notification.Event.User.IsCurrent)
            return;
        if (notification.Event.Emoji.Name != _reaction.Name)
            return;
        var match = PreparedResponses.FirstOrDefault(r => !r.Used &&  r.MessageId == notification.Event.Message.Id);
        if (match is null)
            return;
        await notification.Event.Message.RespondAsync($"{Formatter.Bold(match.Key)}: {match.Definition}");
        match.Used = true;
    }
}