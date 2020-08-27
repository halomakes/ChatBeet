using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using static MoreLinq.Extensions.RepeatExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace ChatBeet.Rules
{
    public class SlotMachineRule : MessageRuleBase<PrivateMessage>
    {
        private static readonly List<List<string>> options;
        private readonly Regex filter;

        public SlotMachineRule(IOptions<IrcBotConfiguration> options)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.CommandPrefix)}(slot)$", RegexOptions.IgnoreCase);
        }

        static SlotMachineRule()
        {
            var baseOptions = new List<string> { "7️⃣", "🔔", "🧲", "🍒", "🍋", "🍊", "🍉", "💰", "💎" };
            var opts = baseOptions.ToSingleElementSequence().Repeat(3).ToList();
            opts = AppendSequence(opts, "WIN");
            opts = AppendSequence(opts, "BAR");
            options = opts;
        }

        private static List<List<string>> AppendSequence(List<List<string>> list, string sequence)
        {
            if (sequence.Length != list.Count())
                throw new ArgumentException("String and list sizes must match.");
            return list.Zip(sequence.ToCharArray(), (list, @char) => list.Append(@char.ToString()).ToList()).ToList();
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                var result = string.Join(string.Empty, options.Select(ol => ol.PickRandom()));
                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From}: {result}");
            }
        }
    }
}
