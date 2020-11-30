using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using Humanizer;
using System;

namespace ChatBeet.Commands
{
    public class SpeedometerCommandProcessor : CommandProcessor
    {
        private readonly SpeedometerService speedoService;

        public SpeedometerCommandProcessor(SpeedometerService speedoService)
        {
            this.speedoService = speedoService;
        }

        [Command("speed {timeSpan}", Description = "Get the message rate in the current channel")]
        [Command("chatrate {timeSpan}", Description = "Get the message rate in the current channel")]
        [ChannelOnly]
        public IClientMessage GetMessageRate(string timeSpan)
        {
            var period = TimeSpan.TryParse(timeSpan, out var ts) ? ts : TimeSpan.FromMinutes(1);
            var rate = speedoService.GetRecentMessageCount(IncomingMessage.To, period);
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"There {(rate == 1 ? "has" : "have")} been {rate} {(rate == 1 ? "message" : "messages")} in {IncomingMessage.To} in the last {period.Humanize()}");
        }
    }
}
