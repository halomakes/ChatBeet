using ChatBeet;
using DtellaRules.Models;
using Humanizer;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace DtellaRules.Rules
{
    public class DownloadCompleteRule : MessageRuleBase<DownloadCompleteMessage>
    {
        private readonly ChatBeetConfiguration config;

        public DownloadCompleteRule(IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(DownloadCompleteMessage incomingMessage)
        {
            if (incomingMessage.Source == "deluge" && !string.IsNullOrEmpty(incomingMessage.Name))
            {
                yield return new OutboundIrcMessage
                {
                    Content = $"{IrcValues.BOLD}{IrcValues.LIME}Download Complete{IrcValues.RESET}: {incomingMessage.Name}",
                    Target = config.NotifyChannel
                };
            }
        }
    }
}
