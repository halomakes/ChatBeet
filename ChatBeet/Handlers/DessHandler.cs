using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands.Discord;
using ChatBeet.Notifications;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Handlers;

public partial class DessHandler : INotificationHandler<DiscordNotification<MessageCreateEventArgs>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DessHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(DiscordNotification<MessageCreateEventArgs> notification,
        CancellationToken cancellationToken)
    {
        if (!Rgx().IsMatch(notification.Event.Message.Content))
            return;

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var booru = scope.ServiceProvider.GetRequiredService<BooruService>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var commandModule = new BooruCommandModule(booru, mediator);
        var (text, embed) = await commandModule.GetResponseContent("akatsuki_kirika", true,
            notification.Event.Author.DiscriminatedUsername());
        var builder = new DiscordMessageBuilder()
            .WithContent(text);
        if (embed is not null)
            builder = builder.WithEmbed(embed);
        await notification.Event.Message.RespondAsync(builder);
    }

    [GeneratedRegex(@"^!dess$", RegexOptions.IgnoreCase)]
    private static partial Regex Rgx();
}