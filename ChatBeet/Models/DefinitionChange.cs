using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ChatBeet.Models;

public class DefinitionChange : INotification
{
    [Required, MaxLength(250), Display(Prompt = "astolfo")]
    public string Key { get; set; }

    [Required, MaxLength(500), Display(Name = "Value", Prompt = "kyoot")]
    public string NewValue { get; set; }

    public string OldValue { get; set; }

    public string NewNick { get; set; }

    public string OldNick { get; set; }
}