using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetIRC.Messages
{
    public abstract class IRCMessage
    {
        private static readonly Dictionary<string, Type> messageMapping;
        static IRCMessage()
        {
            messageMapping = new Dictionary<string, Type>
            {
                { "PING", typeof(PingMessage) },
                { "PRIVMSG", typeof(PrivMsgMessage) },
                { "NOTICE", typeof(NoticeMessage) },
                { "NICK", typeof(NickMessage) },
                { "JOIN", typeof(JoinMessage) },
                { "PART", typeof(PartMessage) },
                { "QUIT", typeof(QuitMessage) },
                { "KICK", typeof(KickMessage) },
                { "001", typeof(RplWelcomeMessage) },
                { "002", typeof(RplYourHostMessage) },
                { "003", typeof(RplCreatedMessage) },
                { "004", typeof(RplMyInfoMessage) },
                { "005", typeof(RplISupportMessage) },
                { "353", typeof(RplNamReplyMessage) },
            };
        }

        // Not using reflection yet because of .NET Standard
        // Will be updated when .NET Standard 2.0 gets released
        public static IServerMessage Create(ParsedIRCMessage parsedMessage)
        {
            var messageType = messageMapping.ContainsKey(parsedMessage.Command)
                ? messageMapping[parsedMessage.Command]
                : null;

            return messageType != null
                ? Activator.CreateInstance(messageType, new object[] { parsedMessage }) as IServerMessage
                : null;
        }

        public override string ToString()
        {
            var clientMessage = this as IClientMessage;

            if (clientMessage == null)
            {
                return base.ToString();
            }

            var tokens = clientMessage.Tokens.ToArray();

            if (tokens.Length == 0)
            {
                return string.Empty;
            }

            var lastIndex = tokens.Length - 1;

            var sb = new StringBuilder();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (i == lastIndex && tokens[i].Contains(" "))
                {
                    sb.Append(':');
                }

                sb.Append(tokens[i]);

                if (i < lastIndex)
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString().Trim();
        }
    }
}
