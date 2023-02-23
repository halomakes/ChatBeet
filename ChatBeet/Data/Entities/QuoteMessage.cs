namespace ChatBeet.Data.Entities;

public class QuoteMessage
{
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AuthorId { get; set; }
    public int Embeds { get; set; }
    public int Attachments { get; set; }

    public virtual User? Author { get; set; }
}