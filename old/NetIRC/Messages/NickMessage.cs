using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class NickMessage : IRCMessage, IServerMessage, IClientMessage
    {
        public string OldNick { get; }
        public string NewNick { get; }

        public NickMessage(ParsedIRCMessage parsedMessage)
        {
            OldNick = parsedMessage.Prefix.From;
            NewNick = parsedMessage.Parameters[0];
        }

        public NickMessage(string newNick)
        {
            NewNick = newNick;
        }

        public IEnumerable<string> Tokens => new[] { "NICK", NewNick };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnNick(new IRCMessageEventArgs<NickMessage>(this));
        }
    }
}
