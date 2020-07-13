using ChatBeet;
using ChatBeet.Irc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class KerningRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly MessageQueueService messageQueueService;

        public KerningRule(MessageQueueService messageQueueService, IOptions<ChatBeetConfiguration> options)
        {
            this.messageQueueService = messageQueueService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($@"^{config.CommandPrefix}kern ([A-z0-9-\[\]\\\^\{{\}}]*)$");
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var nick = match.Groups[1].Value;
                var message = messageQueueService.GetLatestMessage(nick, incomingMessage);

                if (message != null)
                {
                    var spaced = string.Join(" ", message.Content.ToCharArray()).ToUpper();

                    yield return new OutboundIrcMessage
                    {
                        Content = $"<{nick}> {spaced}",
                        Target = incomingMessage.Channel
                    };
                }
            }
        }
    }
}