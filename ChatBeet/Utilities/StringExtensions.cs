namespace ChatBeet.Utilities;

public static class StringExtensions
{
    public static string? CapitalizeFirst(this string? input) => input switch
    {
        null => null,
        "" => input,
        _ => input.First().ToString().ToUpper() + input.Substring(1).ToLower()
    };

    public static string ToPossessive(this string name) => $"{name}{name.GetPossessiveSuffix()}";

    public static string GetPossessiveSuffix(this string name) => name.EndsWith('s') ? "'" : "'s";
}