using System.Collections.Generic;
using System.Globalization;

namespace ChatBeet.Configuration
{
    public class DtellaRuleConfiguration
    {
        public TwitterConfiguration Twitter { get; set; }
        public LastFmConfiguration LastFm { get; set; }
        public PixivConfiguration Pixiv { get; set; }
        public Dictionary<string, string> Urls { get; set; }
        public static CultureInfo Culture = new CultureInfo("en-US");

        public class TwitterConfiguration
        {
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string AccessKey { get; set; }
            public string AccessSecret { get; set; }
        }

        public class LastFmConfiguration
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

        public class PixivConfiguration
        {
            public string UserId { get; set; }
            public string Password { get; set; }
        }
    }
}
