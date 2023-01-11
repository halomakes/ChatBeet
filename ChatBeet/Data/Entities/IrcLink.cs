using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities;
public class IrcLink
{
    [Key]
    public ulong Id { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Discriminator { get; set; }
    [Required]
    public string Nick { get; set; }
}