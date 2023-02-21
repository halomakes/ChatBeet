using ChatBeet.Data.Entities;

namespace ChatBeet.Models;

public class PreferenceChangeRequest
{
    public UserPreference Preference { get; init; }
    public string Value { get; init; }
}