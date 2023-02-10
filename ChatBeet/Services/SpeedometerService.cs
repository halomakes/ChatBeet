using ChatBeet.Utilities;
using System.Collections.Generic;

namespace ChatBeet.Services;

public class SpeedometerService
{
    private static readonly Dictionary<ulong, SlidingBuffer<DateTime>> History = new();
    private const int MaxMessages = 500;

    public static void RecordMessage(ulong channelId)
    {
        if (!History.ContainsKey(channelId))
            History[channelId] = new SlidingBuffer<DateTime>(MaxMessages);

        History[channelId].Add(DateTime.Now);
    }

    public static int GetRecentMessageCount(ulong channel, TimeSpan timeSpan)
    {
        var deadline = DateTime.Now - timeSpan;
        return !History.ContainsKey(channel) 
            ? 0 
            : History[channel].Count(d => d >= deadline);
    }
}