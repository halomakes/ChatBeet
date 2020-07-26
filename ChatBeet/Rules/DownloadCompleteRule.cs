using ChatBeet.Models;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class DownloadCompleteRule : MessageRuleBase<DownloadCompleteMessage>
    {
        private readonly ChatBeetConfiguration config;

        public DownloadCompleteRule(IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(DownloadCompleteMessage incomingMessage)
        {
            if (incomingMessage.Source == "deluge" && !string.IsNullOrEmpty(incomingMessage.Name))
            {
                var downloadTitle = incomingMessage.Name;
                downloadTitle = new Regex(@"(\[.*?\])").Replace(downloadTitle, $"{IrcValues.ORANGE}$1{IrcValues.RESET}");
                downloadTitle = new Regex(@"(\.[A-z0-9]{3})$").Replace(downloadTitle, $"{IrcValues.GREY}$1{IrcValues.RESET}");

                yield return new PrivateMessage(config.NotifyChannel, $"{IrcValues.BOLD}{IrcValues.LIME}Download Complete{IrcValues.RESET}: {downloadTitle}");
            }
        }
    }
}
