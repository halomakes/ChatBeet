using DSharpPlus.Entities;
using MediatR;

namespace ChatBeet.Notifications;

public record KarmaChangeNotification(DiscordMessage TriggeringMessage, string Subject, int NewValue, int OldValue) : INotification;