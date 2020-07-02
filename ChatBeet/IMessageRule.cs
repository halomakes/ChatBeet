using System.Collections.Generic;
using System.Linq;

namespace ChatBeet
{
    public interface IMessageRule
    {
        IAsyncEnumerable<OutboundIrcMessage> Respond(IInboundMessage incomingMessage);
    }

    public interface IMessageRule<TMessage> : IMessageRule where TMessage : IInboundMessage
    {
        IAsyncEnumerable<OutboundIrcMessage> Respond(TMessage incomingMessage);
    }

    public abstract class MessageRuleBase<TMessage> : IMessageRule<TMessage>, IMessageRule where TMessage : IInboundMessage
    {
        public abstract IAsyncEnumerable<OutboundIrcMessage> Respond(TMessage incomingMessage);

        public IAsyncEnumerable<OutboundIrcMessage> Respond(IInboundMessage incomingMessage) =>
            incomingMessage is TMessage message
            ? Respond(message)
            : EmptyResult();

        protected IAsyncEnumerable<OutboundIrcMessage> EmptyResult() => AsyncEnumerable.Empty<OutboundIrcMessage>();
    }
}
