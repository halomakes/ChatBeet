using System.Collections.Generic;

namespace ChatBeet.Smtp
{
    public class SmtpListenerConfiguration
    {
        public IEnumerable<int> Ports { get; set; }
        public bool UseAuth { get; set; }
        public AuthConfig AuthConfig { get; set; }
        public string ServerName { get; set; } = "localhost";
    }
}
