using GravyBot.Commands;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Commands.Irc;

public class HighGroundCommandProcessor : CommandProcessor
{
    public static readonly Dictionary<string, string> HighestNicks = new();

    [Command("jump", Description = "Claim the high ground.")]
    [Command("climb", Description = "Claim the high ground.")]
    [ChannelOnly]
    [RateLimit(5, TimeUnit.Minute)]
    public IClientMessage Claim()
    {
        var chan = IncomingMessage.To;
        var nick = IncomingMessage.From;

        if (!HighestNicks.ContainsKey(chan))
        {
            HighestNicks[chan] = nick;
            return new PrivateMessage(chan, $"{nick} has the high ground.");
        }
        else if (nick == HighestNicks[chan])
        {
            HighestNicks.Remove(chan);
            return new PrivateMessage(chan, $"{nick} trips and falls off the high ground.");
        }
        else
        {
            var oldKing = HighestNicks[chan];
            HighestNicks[chan] = nick;
            return new PrivateMessage(chan, $"It's over, {oldKing}! {nick} has the high ground!");
        }
    }
}