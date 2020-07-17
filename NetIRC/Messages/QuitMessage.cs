using System.Collections.Generic;
using System.Linq;

namespace NetIRC.Messages
{
    public class QuitMessage : IRCMessage, IServerMessage, IClientMessage
    {
        public string Nick { get; }
        public string Message { get; }

        public QuitMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Message = parsedMessage.Trailing;
        }

        public QuitMessage(string message)
        {
            Message = message;
        }

        public IEnumerable<string> Tokens => new[] { "QUIT", Message };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnQuit(new IRCMessageEventArgs<QuitMessage>(this));
        }
    }

    public class KickMessage : IRCMessage, IServerMessage
    {
        public string Channel { get; }
        public string Nick { get; set; }
        public string KickedBy { get; set; }

        public KickMessage(ParsedIRCMessage parsedMessage)
        {
            Channel = parsedMessage.Parameters[0];
            Nick = parsedMessage.Parameters[1];
            KickedBy = parsedMessage.Parameters[2];
        }

        public KickMessage(string channel)
        {
            Channel = channel;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnKick(new IRCMessageEventArgs<KickMessage>(this));
        }
    }
}
