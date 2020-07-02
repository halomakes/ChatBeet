using System;

namespace ChatBeet
{
    public interface IInboundMessage
    {
        string Sender { get; }

        string Content { get; }

        DateTime DateRecieved { get; }
    }
}
