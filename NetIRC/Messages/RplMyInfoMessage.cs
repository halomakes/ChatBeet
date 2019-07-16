namespace NetIRC.Messages
{
    public class RplMyInfoMessage : IRCMessage, IServerMessage
    {
        public string[] Parameters { get; }

        public RplMyInfoMessage(ParsedIRCMessage parsedMessage)
        {
            Parameters = parsedMessage.Parameters;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplMyInfo(new IRCMessageEventArgs<RplMyInfoMessage>(this));
        }
    }
}
