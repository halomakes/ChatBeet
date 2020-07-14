using ChatBeet;
using ChatBeet.Irc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public abstract class NickLookupRule : MessageRuleBase<IrcMessage>
    {
        protected readonly ChatBeetConfiguration config;
        protected readonly MessageQueueService messageQueueService;

        public NickLookupRule(MessageQueueService messageQueueService, IOptions<ChatBeetConfiguration> options)
        {
            this.messageQueueService = messageQueueService;
            config = options.Value;
        }

        protected string CommandName;

        protected abstract IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage, string nick, IrcMessage lookupMessage);

        public override IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            if (!string.IsNullOrEmpty(CommandName))
            {
                var rgx = new Regex($@"^{config.CommandPrefix}{CommandName} ([A-z0-9-\[\]\\\^\{{\}}]*)", RegexOptions.IgnoreCase);
                var match = rgx.Match(incomingMessage.Content);
                if (match.Success)
                {
                    var nick = match.Groups[1].Value;
                    var message = messageQueueService.GetLatestMessage(nick, incomingMessage);

                    if (message != null)
                        return Respond(incomingMessage, nick, message);
                }
            }

            return AsyncEnumerable.Empty<OutboundIrcMessage>();
        }
    }
}