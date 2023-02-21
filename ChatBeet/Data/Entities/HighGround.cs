namespace ChatBeet.Data.Entities;

public class HighGround
{
    public ulong GuildId { get; set; }
    public Guid UserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual User? User { get; set; }
    public virtual Guild? Guild { get; set; }
}