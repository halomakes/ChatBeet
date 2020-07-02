using System;

namespace ChatBeet
{
    public class OutboundIrcMessage
    {
        public string Target { get; set; }

        public string Content { get; set; }

        public IrcMessageType OutputType { get; set; } = IrcMessageType.Message;

        public DateTime DateGenerated { get; set; } = DateTime.Now;
    }
}
