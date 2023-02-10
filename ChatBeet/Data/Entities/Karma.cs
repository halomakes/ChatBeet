namespace ChatBeet.Data.Entities;

public class Karma
{
    public string? Key { get; set; }
    public ulong GuildId { get; set; }
    public int Value { get; set; }

    public virtual Guild? Guild { get; set; }
}