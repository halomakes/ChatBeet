using DSharpPlus.Entities;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Models;

public class IrcLinkRequest
{
    public string Nick { get; set; }
    public DiscordUser User { get; set; }
    [Display(Name = "Authentication Token")]
    public string AuthToken { get; set; }
}