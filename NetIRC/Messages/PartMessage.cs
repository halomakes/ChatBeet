using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class PartMessage : IRCMessage, IServerMessage, IClientMessage
    {
        private string channels;


        public string Nick { get; }
        public string Channel { get; }

        public PartMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        public PartMessage(string channels)
        {
            this.channels = channels;
        }

        public IEnumerable<string> Tokens => new[] { "PART", channels };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPart(new IRCMessageEventArgs<PartMessage>(this));
        }
    }
}
