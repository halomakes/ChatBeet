namespace ChatBeet.Data.Entities;

public class StatEvent
{
    public Guid Id { get; set; }
    public required ulong GuildId { get; set; }
    public required string EventType { get; init; }
    public required Guid TriggeringUserId { get; set; }
    public Guid? TargetedUserId { get; set; }
    public required DateTime OccurredAt { get; set; }
    public string? SampleText { get; set; }
}