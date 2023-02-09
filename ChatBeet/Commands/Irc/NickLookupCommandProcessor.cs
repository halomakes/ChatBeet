using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;

namespace ChatBeet.Commands.Irc;

public abstract class NickLookupCommandProcessor : CommandProcessor
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

    protected virtual IClientMessage Process(string nick, Func<PrivateMessage, IClientMessage> action)
    {
        if (nick.Equals(configuration.Nick, StringComparison.InvariantCultureIgnoreCase))
            return negativeResponseService.GetResponse(IncomingMessage);

        var lookupMessage = nick == "^"
            ? messageQueueService.GetLatestMessage(IncomingMessage.To, IncomingMessage)
            : messageQueueService.GetLatestMessage(nick, IncomingMessage.To, IncomingMessage);

        if (lookupMessage == null)
            return NotFound(nick);

        return action(lookupMessage);
    }

    private IClientMessage NotFound(string nick) => new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Couldn't find a recent message from {IrcValues.BOLD}{nick}{IrcValues.RESET}.");
}