using System.ComponentModel.DataAnnotations;
using ChatBeet.Data.Entities;
using MediatR;

namespace ChatBeet.Models;

public class DefinitionChange : INotification
{
    [Required, MaxLength(250), Display(Prompt = "astolfo")]
    public required string Key { get; set; }
    
    public required ulong GuildId { get; set; }

    [Required, MaxLength(500), Display(Name = "Value", Prompt = "kyoot")]
    public required string NewValue { get; set; }

    public string? OldValue { get; set; }

    public required User NewUser { get; set; }

    public User? OldUser { get; set; }
}