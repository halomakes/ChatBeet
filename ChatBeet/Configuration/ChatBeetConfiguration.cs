using System.Collections.Generic;
using System.Globalization;

namespace ChatBeet.Configuration
{
    public class ChatBeetConfiguration
    {
        public TwitterConfiguration Twitter { get; set; }
        public LastFmConfiguration LastFm { get; set; }
        public PixivConfiguration Pixiv { get; set; }
        public TenorConfiguration Tenor { get; set; }
        public IgdbConfiguration Igdb { get; set; }
        public BooruConfiguration Booru { get; set; }
        public PronounConfiguration Pronouns { get; set; }
        public MessageCollectionConfiguration MessageCollection { get; set; }
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

        public class TenorConfiguration
        {
            public string ApiKey { get; set; }
            public int QueryLimit { get; set; } = 10;
        }

        public class IgdbConfiguration
        {
            public string ApiKey { get; set; }
        }

        public class BooruConfiguration
        {
            public IEnumerable<string> BlacklistedTags { get; set; }
        }

        public class PronounConfiguration
        {
            public AllowedPronounsConfiguration Allowed { get; set; }

            public class AllowedPronounsConfiguration
            {
                public IEnumerable<string> Subjects { get; set; }
                public IEnumerable<string> Objects { get; set; }
                public IEnumerable<string> Possessives { get; set; }
                public IEnumerable<string> Reflexives { get; set; }
            }
        }

        public class MessageCollectionConfiguration
        {
            public IEnumerable<string> AllowedChannels { get; set; }
        }
    }
}
