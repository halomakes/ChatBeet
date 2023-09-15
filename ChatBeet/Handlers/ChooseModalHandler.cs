using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands;
using ChatBeet.Notifications;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;

namespace ChatBeet.Handlers;

public class ChooseModalHandler : INotificationHandler<DiscordNotification<ModalSubmitEventArgs>>
{
    private const string ChosenEmoji = "✅";
    private const string RejectedEmoji = "❌";

    public async Task Handle(DiscordNotification<ModalSubmitEventArgs> notification,
        CancellationToken cancellationToken)
    {
        if (notification.Event.Interaction.Data.CustomId != ChooseCommandModule.Id)
            return;

        var optionsString = notification.Event.Values["options"];
        var options = optionsString.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var selectedIndex = Random.Shared.Next(0, options.Length - 1);

        var formattedOptions = options.Select((option, index) => index == selectedIndex
            ? $"{ChosenEmoji} {Formatter.Bold(option)}"
            : $"{RejectedEmoji} {option}"
        );

        await notification.Event.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent(string.Join(Environment.NewLine, formattedOptions)));
    }
}