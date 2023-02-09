using ChatBeet.Models;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Rules;

public class IrcLinkRule : IMessageRule<IrcLinkRequest>
{
    public IEnumerable<IClientMessage> Respond(IrcLinkRequest incomingMessage)
    {
        yield return new PrivateMessage(incomingMessage.Nick, $"Discord user {incomingMessage.User.DiscriminatedUsername()} (https://discordapp.com/users/{incomingMessage.User.Id}) is attempting to link to this IRC account.  Enter the following token into the form in Discord to confirm this action.");
        yield return new PrivateMessage(incomingMessage.Nick, incomingMessage.AuthToken);
    }
}