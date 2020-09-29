using ChatBeet.Services;
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
    public abstract class AsyncNickLookupRule : IAsyncMessageRule<PrivateMessage>
    {
        protected readonly IrcBotConfiguration config;
        protected readonly MessageQueueService messageQueueService;
        protected readonly NegativeResponseService negativeResponseService;
        private readonly Regex rgx;
        protected string CommandName;

        public AsyncNickLookupRule(string commandName, MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options, NegativeResponseService negativeResponseService)
        {
            this.messageQueueService = messageQueueService;
            this.negativeResponseService = negativeResponseService;
            config = options.Value;
            CommandName = commandName;
            new Regex($@"^{Regex.Escape(config.CommandPrefix)}{Regex.Escape(CommandName)} ({RegexUtils.Nick})", RegexOptions.IgnoreCase);
        }

        protected abstract IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage);

        public IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            if (!string.IsNullOrEmpty(CommandName))
            {
                var match = rgx.Match(incomingMessage.Message);
                if (match.Success)
                {
                    var nick = match.Groups[1].Value;
                    if (nick.Equals(config.Nick, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return negativeResponseService.GetResponse(incomingMessage).ToAsyncSingleElementSequence();
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

        public bool Matches(PrivateMessage incomingMessage) => !string.IsNullOrEmpty(CommandName) && rgx.IsMatch(incomingMessage.Message);
    }
}