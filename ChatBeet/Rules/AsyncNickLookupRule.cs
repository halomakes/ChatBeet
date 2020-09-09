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
    public abstract class AsyncNickLookupRule : AsyncMessageRuleBase<PrivateMessage>
    {
        protected readonly IrcBotConfiguration config;
        protected readonly MessageQueueService messageQueueService;
        protected string CommandName;

        public AsyncNickLookupRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options)
        {
            this.messageQueueService = messageQueueService;
            config = options.Value;
        }

        public override bool Matches(PrivateMessage incomingMessage) => BuildRegex().IsMatch(incomingMessage.Message);

        protected abstract IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage);

        public override IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            if (!string.IsNullOrEmpty(CommandName))
            {
                var match = BuildRegex().Match(incomingMessage.Message);
                if (match.Success)
                {
                    var nick = match.Groups[1].Value;
                    if (nick.Equals(config.Nick, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From}: no u").ToAsyncSingleElementSequence();
                    }
                    else
                    {
                        var message = messageQueueService.GetLatestMessage(nick, incomingMessage.To, incomingMessage);

                        if (message != null)
                        {
                            return RespondAsync(incomingMessage, nick, message);
                        }

                    }
                }
            }

            return AsyncEnumerable.Empty<IClientMessage>();
        }

        private Regex BuildRegex() => new Regex($@"^{Regex.Escape(config.CommandPrefix)}{Regex.Escape(CommandName)} ([A-z0-9-\[\]\\\^\{{\}}]*)", RegexOptions.IgnoreCase);
    }
}