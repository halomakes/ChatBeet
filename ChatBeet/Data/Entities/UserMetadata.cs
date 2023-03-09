namespace ChatBeet.Data.Entities;

public class UserMetadata
{
    public Guid UserId { get; set; }
    public Guid AuthorId { get; set; }
    public ulong GuildId { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }
    
    public virtual User? User { get; set; }
    public virtual User? Author { get; set; }
}