namespace ChatBeet.Utilities;

public static class EnumerationExtensions
{
    public static TAttribute GetAttribute<TAttribute, TEnum>(TEnum @enum) where TEnum : Enum where TAttribute : Attribute
    {
        var enumType = typeof(TEnum);
        var memberInfos = enumType.GetMember(@enum.ToString());
        var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
        var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(TAttribute), false);
        return (TAttribute)valueAttributes[0];
    }
}