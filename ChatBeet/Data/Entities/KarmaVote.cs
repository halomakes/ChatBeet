namespace ChatBeet.Data.Entities;

public class KarmaVote
{
    public Guid Id { get; set; }
    public string? Key { get; set; }
    public ulong GuildId { get; set; }
    public Guid VoterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public VoteType Type { get; set; }

    public virtual Guild? Guild { get; set; }
    public virtual User? Voter { get; set; }

    public enum VoteType
    {
        Up,
        Down
    }
}