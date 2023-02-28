using DSharpPlus;
using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Notifications;

public record DiscordNotification<TEvent>(TEvent Event, DiscordClient Client) : INotification where TEvent : DiscordEventArgs;