using DSharpPlus.Entities;
using MediatR;

namespace ChatBeet.Notifications;

public record BonkableMessageNotification(DiscordMessage Message) : INotification;