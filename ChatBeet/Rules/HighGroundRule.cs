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
        private static string highestNick = null;

        public HighGroundRule(IOptions<IrcBotConfiguration> options)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.CommandPrefix)}((climb)|(jump)|(high ground))$", RegexOptions.IgnoreCase);
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                if (string.IsNullOrEmpty(highestNick))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From} has the high ground.");
                    highestNick = incomingMessage.From;
                }
                else if (incomingMessage.From == highestNick)
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From} trips and falls off the high ground.");
                    highestNick = null;
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"It's over, {highestNick}! {incomingMessage.From} has the high ground!");
                    highestNick = incomingMessage.From;
                }
            }
        }
    }
}
