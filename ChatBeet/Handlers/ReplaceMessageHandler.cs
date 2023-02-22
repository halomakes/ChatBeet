using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands;
using ChatBeet.Notifications;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public class ReplaceMessageHandler : INotificationHandler<DiscordNotification<ModalSubmitEventArgs>>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReplaceMessageHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Handle(DiscordNotification<ModalSubmitEventArgs> notification, CancellationToken cancellationToken)
    {
        if (!notification.Event.Interaction.Data.CustomId.StartsWith(MessageTransformCommandModule.Prefix))
            return;
        if (!ulong.TryParse(notification.Event.Interaction.Data.CustomId.AsSpan(MessageTransformCommandModule.Prefix.Length), out var messageId))
            return;

        var message = await notification.Event.Interaction.Channel.GetMessageAsync(messageId);
        var content = message.Content;
        var pattern = notification.Event.Values["pattern"];
        var value = notification.Event.Values["value"];
        var useRegex = notification.Event.Values["regex"] == "true";
        var ignoreCase = notification.Event.Values["ignoreCase"] == "true";

        string replaced;
        if (useRegex)
        {
            try
            {
                var regex = ignoreCase
                    ? new Regex(pattern, RegexOptions.IgnoreCase)
                    : new Regex(pattern);
                replaced = regex.Replace(content, value);
            }
            catch
            {
                await notification.Event.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Regular expression failed")
                );
                return;
            }
        }
        else
        {
            replaced = ignoreCase
                ? content.Replace(pattern, value, StringComparison.InvariantCultureIgnoreCase)
                : content.Replace(pattern, value);
        }

        await notification.Event.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{Formatter.Mention(message.Author)}: {replaced}"));
    }
}