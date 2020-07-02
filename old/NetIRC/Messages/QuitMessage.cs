using System.Collections.Generic;

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
}
