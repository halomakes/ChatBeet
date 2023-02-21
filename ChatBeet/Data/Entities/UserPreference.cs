using ChatBeet.Attributes;

namespace ChatBeet.Data.Entities;

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
    ReflexivePronoun,
    [Parameter("location:weather", "ZIP Code (Weather)")]
    WeatherLocation,
    [Parameter("unit:weather:temperature", "Unit (Weather Temperature)")]
    WeatherTempUnit,
    [Parameter("unit:weather:wind", "Unit (Wind speed)")]
    WeatherWindUnit,
    [Parameter("unit:weather:precip", "Unit (Precipitation)")]
    WeatherPrecipUnit,
    [Parameter("color:gear", "Gear Color")]
    GearColor
}