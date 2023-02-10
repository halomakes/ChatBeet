namespace ChatBeet.Data.Entities;

/// <summary>
/// Stores a definition
/// </summary>
public class Definition
{
    public ulong GuildId { get; set; }
    
    /// <summary>
    /// Key of definition
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Value of definition
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Person who set definition
    /// </summary>
    public Guid CreatedBy { get; set; }
    
    public User? Author { get; set; }
    
    public virtual Guild? Guild { get; set; }
}