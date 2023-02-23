using DSharpPlus.Entities;
using MediatR;

namespace ChatBeet.Notifications;

public record BonkNotification(ulong GuildId, DiscordUser Bonker, DiscordUser Bonkee) : INotification;