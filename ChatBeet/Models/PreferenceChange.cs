using ChatBeet.Data.Entities;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace ChatBeet.Models;

public class PreferenceChange : INotification
{
    [Required]
    public UserPreference? Preference { get; set; }

    public required string? Value { get; set; }

    public required User User { get; set; }
}