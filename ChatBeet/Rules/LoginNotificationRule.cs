using ChatBeet.Models;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class LoginNotificationRule : IMessageRule<LoginCompleteNotification>
    {
        public IEnumerable<IClientMessage> Respond(LoginCompleteNotification incomingMessage)
        {
            yield return new PrivateMessage(incomingMessage.Nick, $"Notice: Someone logged in as you at {incomingMessage.Time}.");
        }
    }
}
