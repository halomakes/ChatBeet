using System.Collections.Generic;

namespace ChatBeet.Irc
{
    public class IrcBotConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public IEnumerable<string> Channels { get; set; }
        public string Nick { get; set; }
        public string Identity { get; set; }
    }
}
