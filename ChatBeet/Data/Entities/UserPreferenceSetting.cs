using System.Text.Json.Serialization;

namespace ChatBeet.Data.Entities;

public class UserPreferenceSetting
{
    public Guid UserId { get; set; }
    public UserPreference Preference { get; set; }
    public string Value { get; set; }
    
    [JsonIgnore]
    public virtual User? User { get; set; }
}