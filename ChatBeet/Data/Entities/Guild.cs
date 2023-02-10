namespace ChatBeet.Data.Entities;

public class Guild
{
    public ulong Id { get; set; }
    public string? Label { get; set; }
    public DateTime AddedAt { get; set; }
    public Guid AddedBy { get; set; }

    public virtual User? AddedByUser { get; set; }
}