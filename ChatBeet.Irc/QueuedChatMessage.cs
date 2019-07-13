using ChatBeet.Queuing;
using Meebey.SmartIrc4net;
using System;

namespace ChatBeet.Irc
{
    public class QueuedChatMessage : IQueuedMessageSource
    {
        public string Target { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime TimeGenerated { get; set; }

        public static QueuedChatMessage FromChannelMessage(IrcMessageData msg) => new QueuedChatMessage
        {
            Body = msg.Message,
            Source = $"irc:{msg.Nick}",
            Target = msg.Channel,
            TimeGenerated = DateTime.Now,
            Title = msg.Ident
        };
    }
}
