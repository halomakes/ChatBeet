using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities;

public class KeywordHit
{
    public Guid Id { get; set; }

    [Required] public Guid KeywordId { get; set; }
    public Guid UserId { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual User? User { get; set; }
}