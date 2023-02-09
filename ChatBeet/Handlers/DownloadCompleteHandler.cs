using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Configuration;
using ChatBeet.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ChatBeet.Handlers;

public partial class DownloadCompleteHandler : INotificationHandler<DownloadCompleteMessage>
{
    private readonly DiscordBotConfiguration _discordConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static DiscordChannel _notifyChannel;

    public DownloadCompleteHandler(IOptions<DiscordBotConfiguration> discordConfig, IServiceScopeFactory serviceScopeFactory)
    {
        this._discordConfig = discordConfig.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    [GeneratedRegex(@"(@""(\[.*?\])"")")]
    private partial Regex TagRgx();

    [GeneratedRegex(@"(\.[A-z0-9]{3})$")]
    private partial Regex ExtensionRgx();

    public async Task Handle(DownloadCompleteMessage notification, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var discord = scope.ServiceProvider.GetRequiredService<DiscordClient>();
        _notifyChannel ??= await discord.GetChannelAsync(_discordConfig.Channels["Incoming"]);
        var downloadTitle = notification.Name;
        downloadTitle = TagRgx().Replace(downloadTitle, string.Empty);
        downloadTitle = ExtensionRgx().Replace(downloadTitle, string.Empty);
        await discord.SendMessageAsync(_notifyChannel, $"{Formatter.Bold("Download complete")}: {downloadTitle}");
    }
}