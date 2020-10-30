using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;

namespace ChatBeet.Commands
{
    public class NickLookupCommandProcessor : CommandProcessor
    {
        private readonly MessageQueueService messageQueueService;
        private readonly NegativeResponseService negativeResponseService;
        private readonly IrcBotConfiguration configuration;

        public NickLookupCommandProcessor(MessageQueueService messageQueueService, NegativeResponseService negativeResponseService, IOptions<IrcBotConfiguration> options)
        {
            this.messageQueueService = messageQueueService;
            this.negativeResponseService = negativeResponseService;
            configuration = options.Value;
        }

        protected IClientMessage Process(string nick, Func<string, string> transformer)
        {
            if (nick.Equals(configuration.Nick, StringComparison.InvariantCultureIgnoreCase))
                return negativeResponseService.GetResponse(IncomingMessage);

            var lookupMessage = GetLatestMessage(nick);

            if (lookupMessage == null)
                return NotFound(nick);

            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {transformer(lookupMessage.Message)}");
        }

        private PrivateMessage GetLatestMessage(string nick) => messageQueueService.GetLatestMessage(nick, IncomingMessage.To, IncomingMessage);

        private IClientMessage NotFound(string nick) => new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Couldn't find a recent message from {IrcValues.BOLD}{nick}{IrcValues.RESET}.");
    }
}
