using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class HighGroundRule : MessageRuleBase<PrivateMessage>
    {
        private readonly Regex filter;
        private static readonly Dictionary<string, string> highestNicks = new Dictionary<string, string>();

        public HighGroundRule(IOptions<IrcBotConfiguration> options)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.CommandPrefix)}((climb)|(jump)|(high ground))$", RegexOptions.IgnoreCase);
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                if (!highestNicks.ContainsKey(incomingMessage.To))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From} has the high ground.");
                    highestNicks[incomingMessage.To] = incomingMessage.From;
                }
                else if (incomingMessage.From == highestNicks[incomingMessage.To])
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From} trips and falls off the high ground.");
                    highestNicks.Remove(incomingMessage.To);
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"It's over, {highestNicks[incomingMessage.To]}! {incomingMessage.From} has the high ground!");
                    highestNicks[incomingMessage.To] = incomingMessage.From;
                }
            }
        }
    }
}
