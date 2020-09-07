using ChatBeet.Annotations;

namespace ChatBeet.Data.Entities
{
    public enum UserPreference
    {
        [Parameter("birthday", "Date of Birth"),]
        DateOfBirth,
        [Parameter("pronouns", "Pronouns")]
        Pronouns,
        [Parameter("workstart", "Start of Working Hours")]
        WorkHoursStart,
        [Parameter("workend", "End of Working Hours")]
        WorkHoursEnd
    }
}
