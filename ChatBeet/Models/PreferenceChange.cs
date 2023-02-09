using ChatBeet.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Models;

public class PreferenceChange
{
    [Required]
    public UserPreference? Preference { get; set; }

    public string Value { get; set; }

    public string Nick { get; set; }
}