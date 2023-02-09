using ChatBeet.Utilities;
using GravyBot;
using System;
using System.Linq;

namespace ChatBeet.Services;

public class SpeedometerService
{
    private readonly MessageQueueService messageQueue;

    public SpeedometerService(MessageQueueService messageQueue)
    {
        this.messageQueue = messageQueue;
    }

    public int GetRecentMessageCount(string channelName, TimeSpan timeSpan)
    {
        var deadline = DateTime.Now - timeSpan;
        return messageQueue.GetChatLog()
            .Where(m => m.To.EqualsIgnoreCase(channelName))
            .Count(m => m.DateReceived >= deadline);
    }
}