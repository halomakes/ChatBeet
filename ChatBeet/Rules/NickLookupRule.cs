using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public abstract class NickLookupRule : MessageRuleBase<PrivateMessage>
    {
        protected readonly IrcBotConfiguration config;
        protected readonly MessageQueueService messageQueueService;
        protected string CommandName;

        public NickLookupRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options)
        {
            this.messageQueueService = messageQueueService;
            config = options.Value;
        }

        protected abstract IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage);

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (!string.IsNullOrEmpty(CommandName))
            {
                var rgx = new Regex($@"^{Regex.Escape(config.CommandPrefix)}{Regex.Escape(CommandName)} ([A-z0-9-\[\]\\\^\{{\}}]*)", RegexOptions.IgnoreCase);
                var match = rgx.Match(incomingMessage.Message);
                if (match.Success)
                {
                    var nick = match.Groups[1].Value;
                    if (nick.Equals(config.Nick, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From}: no u").ToSingleElementSequence();
                    }
                    else
                    {
                        var message = messageQueueService.GetLatestMessage(nick, incomingMessage.To, incomingMessage);

                        if (message != null)
                        {
                            return Respond(incomingMessage, nick, message);
                        }
                        else
                        {
                            return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Couldn't find a recent message from {IrcValues.BOLD}{nick}{IrcValues.RESET}.").ToSingleElementSequence();
                        }
                    }
                }
            }

            return Enumerable.Empty<IClientMessage>();
        }
    }
}