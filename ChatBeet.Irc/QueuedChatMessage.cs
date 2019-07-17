using ChatBeet.Queuing;
using NetIRC.Messages;
using System;
using System.Linq;

namespace ChatBeet.Irc
{
    public class QueuedChatMessage : IQueuedMessageSource
    {
        public string Target { get; set; }
        public string Source { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime TimeGenerated { get; set; }

        public static QueuedChatMessage FromChannelMessage(PrivMsgMessage msg) => new QueuedChatMessage
        {
            Body = msg.Message,
            Source = $"irc:{msg.From}",
            Target = msg.To,
            TimeGenerated = DateTime.Now,
            Title = msg.Tokens.FirstOrDefault() ?? string.Empty
        };
    }
}
