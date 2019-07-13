using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBeet.Irc
{
    public class IrcBotConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Channel { get; set; }
        public string Nick { get; set; }
        public string Identity { get; set; }
    }
}
