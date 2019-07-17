namespace NetIRC.Messages
{
    public class PingMessage : IRCMessage, IServerMessage
    {
        public string Target { get; }

        public PingMessage(ParsedIRCMessage parsedMessage)
        {
            Target = parsedMessage.Trailing ?? parsedMessage.Parameters[0];
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPing(new IRCMessageEventArgs<PingMessage>(this));
        }
    }
}
