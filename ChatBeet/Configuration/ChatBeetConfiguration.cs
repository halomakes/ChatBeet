﻿using System.Collections.Generic;
using System.Globalization;

namespace ChatBeet.Configuration
{
    public class ChatBeetConfiguration
    {
        public TwitterConfiguration Twitter { get; set; } = new TwitterConfiguration();
        public LastFmConfiguration LastFm { get; set; } = new LastFmConfiguration();
        public PixivConfiguration Pixiv { get; set; } = new PixivConfiguration();
        public TenorConfiguration Tenor { get; set; } = new TenorConfiguration();
        public IgdbConfiguration Igdb { get; set; } = new IgdbConfiguration();
        public BooruConfiguration Booru { get; set; } = new BooruConfiguration();
        public PronounConfiguration Pronouns { get; set; } = new PronounConfiguration();
        public IEnumerable<string> NegativeResponses { get; set; }
        public MessageCollectionConfiguration MessageCollection { get; set; } = new MessageCollectionConfiguration();
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
            public IEnumerable<string> BlacklistedTags { get; set; } = new List<string>();
        }

        public class PronounConfiguration
        {
            public AllowedPronounsConfiguration Allowed { get; set; } = new AllowedPronounsConfiguration();

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
            public IEnumerable<string> AllowedChannels { get; set; } = new List<string>();
        }
    }
}
