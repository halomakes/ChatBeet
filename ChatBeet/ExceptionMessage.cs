using System;

namespace ChatBeet
{
    public class ExceptionMessage : IInboundMessage
    {
        public ExceptionMessage(Exception ex)
        {
            Sender = ex.Source;
            Content = ex.Message;
            Exception = ex;
        }

        public string Sender { get; }

        public string Content { get; }

        public DateTime DateRecieved { get; } = DateTime.Now;

        public Exception Exception { get; }
    }
}
