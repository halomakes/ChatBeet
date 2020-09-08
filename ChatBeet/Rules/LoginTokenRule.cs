using ChatBeet.Models;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class LoginTokenRule : MessageRuleBase<LoginTokenRequest>
    {
        public override IEnumerable<IClientMessage> Respond(LoginTokenRequest incomingMessage)
        {
            yield return new PrivateMessage(incomingMessage.Nick, $"Your login token is {IrcValues.AQUA}{incomingMessage.AuthToken}{IrcValues.RESET}");
        }
    }
}
