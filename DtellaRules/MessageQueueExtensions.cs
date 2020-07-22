using ChatBeet.Irc;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;

namespace DtellaRules
{
    public static class MessageQueueExtensions
    {
        public static IEnumerable<PrivateMessage> GetChatLog(this MessageQueueService messageQueue) => messageQueue.GetHistory()
            .Where(m => m is PrivateMessage)
            .Cast<PrivateMessage>();

        public static PrivateMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, string channel) => messageQueue.GetChatLog()
            .Where(m => m.To.ToLower() == channel.ToLower())
            .LastOrDefault(m => m.From.ToLower() == nick.ToLower());

        public static PrivateMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, string channel, PrivateMessage triggeringMessage) => messageQueue.GetChatLog()
            .Where(m => m != triggeringMessage)
            .Where(m => m.To.ToLower() == channel.ToLower())
            .LastOrDefault(m => m.From.ToLower() == nick.ToLower());
    }
}