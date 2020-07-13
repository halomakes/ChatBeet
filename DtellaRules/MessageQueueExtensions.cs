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

        public static IrcMessage GetLatestMessage(this MessageQueueService messageQueue, string nick) => messageQueue.GetChatLog()
            .LastOrDefault(m => m.Sender.ToLower() == nick.ToLower());

        public static IrcMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, IrcMessage triggeringMessage) => messageQueue.GetChatLog()
            .Where(m => m != triggeringMessage)
            .LastOrDefault(m => m.Sender.ToLower() == nick.ToLower());
    }
}