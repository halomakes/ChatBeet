namespace ChatBeet.Data.Entities;

public class BlacklistedTag
{
    public Guid UserId { get; set; }

    public string? Tag { get; set; }
    
    public virtual User? User { get; set; }
}