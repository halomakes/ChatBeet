using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class DrawLotsRule : MessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly IMemoryCache cache;
        private static readonly int maxLength = 16;
        private static readonly Random rng = new Random();
        private static readonly int godChance = 10_000_000;
        private static readonly int godLength = 32;

        public DrawLotsRule(IOptions<IrcBotConfiguration> opts, IMemoryCache cache)
        {
            config = opts.Value;
            this.cache = cache;
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}(lots|epeen)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var bar = GetLot(match.Groups[1].Value.Trim().ToLower(), incomingMessage.From);
                if (!string.IsNullOrEmpty(bar))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{bar} {incomingMessage.From}");
                }
            }
        }

        private string GetLot(string mode, string nick)
        {
            var length = cache.GetOrCreate($"lot:{nick}:{mode}", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                var isGodLength = rng.Next(0, godChance) == 0;
                return isGodLength ? godLength : rng.NormalNext(1, maxLength);
            });

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
