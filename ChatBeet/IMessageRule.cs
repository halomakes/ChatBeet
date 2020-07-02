using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet
{
    public interface IMessageRule
    {
        Task<List<OutboundIrcMessage>> Respond(IInboundMessage incomingMessage);
    }

    public interface IMessageRule<TMessage> : IMessageRule where TMessage : IInboundMessage
    {
        Task<List<OutboundIrcMessage>> Respond(TMessage incomingMessage);

        new Task<List<OutboundIrcMessage>> Respond(IInboundMessage incomingMessage) =>
            incomingMessage is TMessage message
            ? Respond(message)
            : Task.FromResult(new List<OutboundIrcMessage>());
    }
}
