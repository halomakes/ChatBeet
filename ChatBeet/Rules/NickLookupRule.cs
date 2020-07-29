using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public abstract class NickLookupRule : MessageRuleBase<PrivateMessage>
    {
        protected readonly IrcBotConfiguration config;
        protected readonly MessageQueueService messageQueueService;

        public NickLookupRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options)
        {
            this.messageQueueService = messageQueueService;
            config = options.Value;
        }

        protected string CommandName;
        private MessageQueueService messageQueueService1;
        private IOptions<IrcBotConfiguration> options;

        protected abstract IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage);

        public override IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (!string.IsNullOrEmpty(CommandName))
            {
                var rgx = new Regex($@"^{config.CommandPrefix}{CommandName} ([A-z0-9-\[\]\\\^\{{\}}]*)", RegexOptions.IgnoreCase);
                var match = rgx.Match(incomingMessage.Message);
                if (match.Success)
                {
                    var nick = match.Groups[1].Value;
                    var message = messageQueueService.GetLatestMessage(nick, incomingMessage.To, incomingMessage);

                    if (message != null)
                    {
                        return Respond(incomingMessage, nick, message);
                    }
                }
            }

            return AsyncEnumerable.Empty<IClientMessage>();
        }
    }
}