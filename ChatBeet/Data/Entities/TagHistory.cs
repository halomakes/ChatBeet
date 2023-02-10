namespace ChatBeet.Data.Entities;

public class TagHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Tag { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual User? User { get; set; }
}