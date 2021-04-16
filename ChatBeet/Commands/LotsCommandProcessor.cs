using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Linq;

namespace ChatBeet.Commands
{
    public class LotsCommandProcessor : CommandProcessor
    {
        private static readonly int maxLength = 16;
        private static readonly Random rng = new();
        private static readonly int godChance = 10_000_000;
        private static readonly int godLength = 32;

        [Command("lots", Description = "Draw a lot to compare with other users.")]
        [RateLimit(2, TimeUnit.Minute)]
        public IClientMessage GetLot()
        {
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{GetBar(GetLength(), '-')} {IncomingMessage.From}");
        }

        [Command("epeen")]
        [RateLimit(2, TimeUnit.Minute)]
        public IClientMessage CompareLength()
        {
            var today = DateTime.Now;
            if(today.Month == 4 && today.Day == 16)
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{GetBar(godLength, '=')} {IncomingMessage.From} Happy national horny day!");
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{GetBar(GetLength(), '=')} {IncomingMessage.From}");
        }

        private int GetLength()
        {
            var isGodLength = rng.Next(0, godChance) == 0;
            return isGodLength ? godLength : rng.NormalNext(1, maxLength);
        }

        private static string GetBar(int length, char @char) => new(Enumerable.Repeat(@char, length).ToArray());
    }
}
