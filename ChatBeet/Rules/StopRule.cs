using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class StopRule : MessageRuleBase<PrivateMessage>
    {
        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex(@"^st(o|ah)p([?!.]+)?$", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Message))
            {
                yield return new PrivateMessage(
                    incomingMessage.GetResponseTarget(),
                    rgx.Replace(incomingMessage.Message, @"W$1it $1 minute$2")
                );
            }
        }
    }
}
