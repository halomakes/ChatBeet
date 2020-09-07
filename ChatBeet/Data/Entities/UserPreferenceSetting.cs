namespace ChatBeet.Data.Entities
{
    public class UserPreferenceSetting
    {
        public string Nick { get; set; }
        public UserPreference Preference { get; set; }
        public string Value { get; set; }
    }
}
