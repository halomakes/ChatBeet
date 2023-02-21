using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Notifications;

public record DiscordNotification<TEvent>(TEvent Event) : INotification where TEvent : DiscordEventArgs;