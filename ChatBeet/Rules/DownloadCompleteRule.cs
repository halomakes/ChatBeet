using ChatBeet.Models;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;

public partial class DownloadCompleteRule : IAsyncMessageRule<DownloadCompleteMessage>
{
    private readonly IrcBotConfiguration config;

    public DownloadCompleteRule(IOptions<IrcBotConfiguration> options)
    {
        config = options.Value;
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
            var downloadTitle = incomingMessage.Name;
            downloadTitle = TagRgx().Replace(downloadTitle, $"{IrcValues.ORANGE}$1{IrcValues.RESET}");
            downloadTitle = ExtensionRgx().Replace(downloadTitle, $"{IrcValues.GREY}$1{IrcValues.RESET}");

            yield return new PrivateMessage(config.NotifyChannel, $"{IrcValues.BOLD}{IrcValues.LIME}Download Complete{IrcValues.RESET}: {downloadTitle}");
        }
    }
}
