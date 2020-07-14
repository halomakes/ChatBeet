using ChatBeet;
using ChatBeet.Irc;
using System.Collections.Generic;
using System.Linq;

namespace DtellaRules
{
    public static class MessageQueueExtensions
    {
        public static IEnumerable<IrcMessage> GetChatLog(this MessageQueueService messageQueue) => messageQueue.GetHistory()
            .Where(m => m is IrcMessage)
            .Cast<IrcMessage>();

        public static IrcMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, string channel) => messageQueue.GetChatLog()
            .Where(m => m.Channel.ToLower() == channel.ToLower())
            .LastOrDefault(m => m.Sender.ToLower() == nick.ToLower());

        public static IrcMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, string channel, IrcMessage triggeringMessage) => messageQueue.GetChatLog()
            .Where(m => m != triggeringMessage)
            .Where(m => m.Channel.ToLower() == channel.ToLower())
            .LastOrDefault(m => m.Sender.ToLower() == nick.ToLower());
    }
}