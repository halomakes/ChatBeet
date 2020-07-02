using System.Collections.Generic;

namespace DtellaRules
{
    public class DtellaRuleConfiguration
    {
        public TwitterConfiguration Twitter { get; set; }
        public Dictionary<string, string> Urls { get; set; }

        public class TwitterConfiguration
        {
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string AccessKey { get; set; }
            public string AccessSecret { get; set; }
        }
    }
}
