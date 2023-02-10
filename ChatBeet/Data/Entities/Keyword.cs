using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChatBeet.Data.Entities;

/// <summary>
/// A keyword to log hits against
/// </summary>
public class Keyword
{
    /// <summary>
    /// Unique ID for the keyword
    /// </summary>
    public Guid Id { get; set; }
    
    public ulong GuildId { get; set; }

    /// <summary>
    /// Label to show on stats page
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Expression to check messages against
    /// </summary>
    public string? Regex { get; set; }

    /// <summary>
    /// Order to show on the stats page
    /// </summary>
    public int SortOrder { get; set; }

    [JsonIgnore]
    public virtual ICollection<KeywordHit>? Hits { get; set; }
    
    public virtual Guild? Guild { get; set; }
}