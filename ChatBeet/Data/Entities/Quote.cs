using System.Collections.Generic;

namespace ChatBeet.Data.Entities;

public class Quote
{
    public ulong GuildId { get; set; }
    public required string Slug { get; set; }
    public Guid SavedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string ChannelName { get; set; }

    public virtual ICollection<QuoteMessage>? Messages { get; set; }
    public virtual User? SavedBy { get; set; }
}