using ChatBeet;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class StopRule : MessageRuleBase<IrcMessage>
    {
        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex(@"^st(o|ah)p([?!.]+)?$", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Content))
            {
                yield return new OutboundIrcMessage
                {
                    Content = rgx.Replace(incomingMessage.Content, @"W$1it $1 minute$2"),
                    Target = incomingMessage.Channel
                };
            }
        }
    }
}
