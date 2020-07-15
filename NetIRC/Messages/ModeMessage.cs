using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class ModeMessage : IRCMessage, IClientMessage
    {
        private readonly string mode;
        private readonly string nick;

        public ModeMessage(string nick, string mode)
        {
            this.mode = mode;
            this.nick = nick;
        }

        public IEnumerable<string> Tokens => new[] { "MODE", nick, mode };
    }
}
