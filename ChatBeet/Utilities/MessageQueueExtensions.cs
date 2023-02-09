using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Utilities;

public static class MessageQueueExtensions
{
    public static IEnumerable<PrivateMessage> GetChatLog(this MessageQueueService messageQueue) => messageQueue.GetHistory()
        .Where(m => m is PrivateMessage)
        .Cast<PrivateMessage>();

    public static IEnumerable<PrivateMessage> GetChatLog(this MessageQueueService messageQueue, string channel) => messageQueue.GetChatLog()
        .Where(m => m.To.EqualsIgnoreCase(channel));

    public static PrivateMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, string channel) => messageQueue.GetChatLog(channel)
        .OrderByDescending(m => m.DateReceived)
        .FirstOrDefault(m => m.From.EqualsIgnoreCase(nick));

    public static PrivateMessage GetLatestMessage(this MessageQueueService messageQueue, string nick, string channel, PrivateMessage triggeringMessage) => messageQueue.GetChatLog(channel)
        .OrderByDescending(m => m.DateReceived)
        .Where(m => m != triggeringMessage)
        .FirstOrDefault(m => m.From.EqualsIgnoreCase(nick));

    public static PrivateMessage GetLatestMessage(this MessageQueueService messageQueue, string channel, PrivateMessage triggeringMessage) => messageQueue.GetChatLog(channel)
        .OrderByDescending(m => m.DateReceived)
        .Where(m => m != triggeringMessage)
        .FirstOrDefault();

    public static PrivateMessage GetLatestMessage(this MessageQueueService messageQueue, string channel, Regex pattern) => messageQueue.GetChatLog(channel)
        .OrderByDescending(m => m.DateReceived)
        .FirstOrDefault(m => pattern.IsMatch(m.Message));
}