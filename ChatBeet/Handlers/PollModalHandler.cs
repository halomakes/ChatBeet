using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands;
using ChatBeet.Notifications;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using MediatR;

namespace ChatBeet.Handlers;

public class PollModalHandler : INotificationHandler<DiscordNotification<ModalSubmitEventArgs>>
{
    private readonly string[] _defaultEmoji = { "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣", "🔟" };

    public async Task Handle(DiscordNotification<ModalSubmitEventArgs> notification, CancellationToken cancellationToken)
    {
        if (notification.Event.Interaction.Data.CustomId != PollCommandModule.Id)
            return;

        var description = notification.Event.Values["description"];
        var optionsString = notification.Event.Values["options"];
        try
        {
            var expiration = TimeSpan.FromMinutes(2);
            var options = ParseOptions(optionsString, notification.Client).DistinctBy(o => o.Key.Name).ToList();
            var leadingText = $@"{Formatter.Mention(notification.Event.Interaction.User)} has started a poll! This will expire {Formatter.Timestamp(expiration)}
{description}

{string.Join(Environment.NewLine, options.Select(o => $"{o.Key}: {o.Value}"))}";
            await notification.Event.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(leadingText));
            var pollStartMessage = await notification.Event.Interaction.GetOriginalResponseAsync();
            var pollResults = await pollStartMessage.DoPollAsync(options.Select(o => o.Key), PollBehaviour.KeepEmojis, expiration);
            var formattedResults = pollResults
                .OrderByDescending(t => t.Total)
                .Join(options, e => e.Emoji.Name, p => p.Key.Name, (e, p) => $"{e.Total} - {e.Emoji}: {p.Value}")
                .ToList();
            await pollStartMessage.ModifyAsync($@"{Formatter.Mention(notification.Event.Interaction.User)} ran a poll: {description}
{string.Join(Environment.NewLine, formattedResults)}");
        }
        catch (IndexOutOfRangeException)
        {
            await notification.Event.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"You must specify emoji for each option when using more than {_defaultEmoji.Length} options."));
        }
    }

    private IEnumerable<KeyValuePair<DiscordEmoji, string>> ParseOptions(string options, DiscordClient client)
    {
        var index = -1;
        foreach (var line in options.Split("\n"))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            index++;
            var firstSpace = line.IndexOf(' ');
            if (firstSpace < 0)
                yield return new(DiscordEmoji.FromUnicode(_defaultEmoji[index]), line);

            var prefix = line[..firstSpace];
            if (DiscordEmoji.TryFromUnicode(prefix, out var unicodeEmoji))
                yield return new(unicodeEmoji, line[firstSpace..].Trim());
            else if (prefix.Length > 2 && prefix.StartsWith(':') && prefix.EndsWith(':') && DiscordEmoji.TryFromName(client, prefix[1..^1], true, out var namedEmoji))
                yield return new(namedEmoji, line[firstSpace..].Trim());
            else
                yield return new(DiscordEmoji.FromUnicode(_defaultEmoji[index]), line);
        }
    }
}