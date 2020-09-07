using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Data.Entities
{
    public enum UserPreference
    {
        [Display(Name = "Date of Birth")]
        DateOfBirth,
        [Display(Name = "Pronouns")]
        Pronouns,
        [Display(Name = "Start of Working Hours")]
        WorkHoursStart,
        [Display(Name = "End of Working Hours")]
        WorkHoursEnd
    }
}
