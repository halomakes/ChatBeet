using ChatBeet.Attributes;

namespace ChatBeet.Data.Entities
{
    public enum UserPreference
    {
        [Parameter("birthday", "Date of Birth"),]
        DateOfBirth,
        [Parameter("pronoun:subject", "Pronoun (Subject)")]
        SubjectPronoun,
        [Parameter("pronoun:object", "Pronoun (Object)")]
        ObjectPronoun,
        [Parameter("work:start", "Start of Working Hours")]
        WorkHoursStart,
        [Parameter("work:end", "End of Working Hours")]
        WorkHoursEnd,
        [Parameter("pronoun:possessive", "Pronoun (Possessive)")]
        PossessivePronoun,
        [Parameter("pronoun:reflexive", "Pronoun (Reflexive)")]
        ReflexivePronoun
    }
}
