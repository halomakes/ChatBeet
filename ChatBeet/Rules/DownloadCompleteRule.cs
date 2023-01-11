using ChatBeet.Configuration;
using ChatBeet.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules;

public partial class DownloadCompleteRule : IAsyncMessageRule<DownloadCompleteMessage>
{
    private readonly IrcBotConfiguration config;
    private readonly DiscordClient discord;
    private readonly DiscordBotConfiguration discordConfig;
    private static DiscordChannel notifyChannel;

    public DownloadCompleteRule(IOptions<IrcBotConfiguration> options, DiscordClient discord, IOptions<DiscordBotConfiguration> discordBotConfiguration)
    {
        config = options.Value;
        this.discord = discord;
        discordConfig = discordBotConfiguration.Value;
    }

    public bool Matches(DownloadCompleteMessage incomingMessage) => true;

    [GeneratedRegex(@"(@""(\[.*?\])"")")]
    private partial Regex TagRgx();

    [GeneratedRegex(@"(\.[A-z0-9]{3})$")]
    private partial Regex ExtensionRgx();

    public async IAsyncEnumerable<IClientMessage> RespondAsync(DownloadCompleteMessage incomingMessage)
    {
        if (incomingMessage.Source == "deluge" && !string.IsNullOrEmpty(incomingMessage.Name))
        {
            await SendDiscordNotificationAsync(incomingMessage);
            var downloadTitle = incomingMessage.Name;
            downloadTitle = TagRgx().Replace(downloadTitle, $"{IrcValues.ORANGE}$1{IrcValues.RESET}");
            downloadTitle = ExtensionRgx().Replace(downloadTitle, $"{IrcValues.GREY}$1{IrcValues.RESET}");

            yield return new PrivateMessage(config.NotifyChannel, $"{IrcValues.BOLD}{IrcValues.LIME}Download Complete{IrcValues.RESET}: {downloadTitle}");
        }
    }

    private async Task SendDiscordNotificationAsync(DownloadCompleteMessage incomingMessage)
    {
        notifyChannel ??= await discord.GetChannelAsync(discordConfig.Channels["Incoming"]);
        var downloadTitle = incomingMessage.Name;
        downloadTitle = TagRgx().Replace(downloadTitle, string.Empty);
        downloadTitle = ExtensionRgx().Replace(downloadTitle, string.Empty);
        await discord.SendMessageAsync(notifyChannel, $"{Formatter.Bold("Download complete")}: {downloadTitle}");
    }
}
