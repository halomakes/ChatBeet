namespace ChatBeet.Data.Entities;

public class SuspicionReport
{
    public Guid Id { get; set; }

    public ulong GuildId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid ReporterId { get; set; }

    public Guid SuspectId { get; set; }

    public virtual User? Reporter { get; set; }

    public virtual User? Suspect { get; set; }
    
    public virtual Guild? Guild { get; set; }
}