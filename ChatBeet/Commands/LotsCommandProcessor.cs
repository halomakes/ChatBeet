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
        private static readonly Random rng = new Random();
        private static readonly int godChance = 10_000_000;
        private static readonly int godLength = 32;

        [Command("lots", Description = "Draw a lot to compare with other users.")]
        [Command("epeen")]
        [RateLimit(5, TimeUnit.Minute)]
        public IClientMessage GetLot()
        {
            var bar = BuildLot(TriggeringCommandName);
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{bar} {IncomingMessage.From}");
        }

        private string BuildLot(string mode)
        {
            var isGodLength = rng.Next(0, godChance) == 0;
            var length = isGodLength ? godLength : rng.NormalNext(1, maxLength);

            return mode switch
            {
                "lots" => GetBar(length, '-'),
                "epeen" => $"8{GetBar(length, '=')}D",
                _ => default
            };
        }

        private static string GetBar(int length, char @char) => new string(Enumerable.Repeat(@char, length).ToArray());
    }
}
