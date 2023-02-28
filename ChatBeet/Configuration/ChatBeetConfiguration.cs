using System.Collections.Generic;
using System.Globalization;
using Untappd.Client;

namespace ChatBeet.Configuration;

public class ChatBeetConfiguration
{
    public TwitterConfiguration Twitter { get; set; } = new();
    public LastFmConfiguration LastFm { get; set; } = new();
    public PixivConfiguration Pixiv { get; set; } = new();
    public TenorConfiguration Tenor { get; set; } = new();
    public IgdbConfiguration Igdb { get; set; } = new();
    public BooruConfiguration Booru { get; set; } = new();
    public PronounConfiguration Pronouns { get; set; } = new();
    public IEnumerable<string> NegativeResponses { get; set; } = null!;
    public MessageCollectionConfiguration MessageCollection { get; set; } = new();
    public UntappdConfig Untappd { get; set; } = null!;
    public Dictionary<string, string> Urls { get; set; } = null!;
    public static CultureInfo Culture = new("en-US");
    public string MainChannel { get; set; } = null!;
    public string Sauce { get; set; } = null!;

    public class TwitterConfiguration
    {
        public string ConsumerKey { get; set; } = null!;
        public string ConsumerSecret { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string AccessSecret { get; set; } = null!;
    }

    public class LastFmConfiguration
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }

    public class PixivConfiguration
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class TenorConfiguration
    {
        public string ApiKey { get; set; } = null!;
        public int QueryLimit { get; set; } = 10;
    }

    public class IgdbConfiguration
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }

    public class BooruConfiguration
    {
        public IEnumerable<string> BlacklistedTags { get; set; } = null!;
    }

    public class PronounConfiguration
    {
        public AllowedPronounsConfiguration Allowed { get; set; } = new();

        public class AllowedPronounsConfiguration
        {
            public List<string> Subjects { get; set; } = null!;
            public List<string> Objects { get; set; } = null!;
            public List<string> Possessives { get; set; } = null!;
            public List<string> Reflexives { get; set; } = null!;
        }
    }

    public class MessageCollectionConfiguration
    {
        public IEnumerable<string> AllowedChannels { get; set; } = null!;
    }
}
