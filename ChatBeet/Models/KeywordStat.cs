using ChatBeet.Data.Entities;
using System.Collections.Generic;

namespace ChatBeet.Models
{
    public class KeywordStat
    {
        public Keyword Keyword { get; set; }

        public IEnumerable<UserKeywordStat> Stats { get; set; }

        public class UserKeywordStat
        {
            public string Nick { get; set; }
            public int Hits { get; set; }
            public string Excerpt { get; set; }
        }
    }
}
