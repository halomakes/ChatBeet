using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class NoticeMessage : IRCMessage, IServerMessage, IClientMessage
    {
        public string From { get; }
        public string Target { get; }
        public string Message { get; }

        public NoticeMessage(ParsedIRCMessage parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Target = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;
        }

        public NoticeMessage(string target, string text)
        {
            Target = target;
            Message = text;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnNotice(new IRCMessageEventArgs<NoticeMessage>(this));
        }

        public IEnumerable<string> Tokens => new[] { "NOTICE", Target, Message };
    }
}
