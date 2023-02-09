using ChatBeet.Data.Entities;
using System.Collections.Generic;

namespace ChatBeet.Models;

/// <summary>
/// Stats about a keyword
/// </summary>
public class KeywordStat
{
    /// <summary>
    /// Keyword associated to the statistics
    /// </summary>
    public Keyword Keyword { get; set; }

    /// <summary>
    /// List of nicks who have used keyword
    /// </summary>
    public IEnumerable<UserKeywordStat> Stats { get; set; }

    /// <summary>
    /// Details about a nick's use of a keyword
    /// </summary>
    public class UserKeywordStat
    {
        /// <summary>
        /// Nick of user
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        /// Number of times user has used keyword
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// Example of user using keyword
        /// </summary>
        public string Excerpt { get; set; }
    }
}