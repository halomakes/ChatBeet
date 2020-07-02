using System;
using System.Collections;

namespace ChatBeet
{
    public class Class1
    {
    }

    public class IrcMessage : IInboundMessage
    {
        public string Sender { get; set; }

        public string Channel { get; set; }

        public string Content { get; set; }

        public DateTime DateRecieved { get; set; }
    }

    public class ExceptionMessage : IInboundMessage
    {
        public ExceptionMessage(Exception ex)
        {
            Sender = ex.Source;
            Content = ex.Message;
        }

        public string Sender { get; }

        public string Content { get; }

        public DateTime DateRecieved { get; } = DateTime.Now;
    }
}
