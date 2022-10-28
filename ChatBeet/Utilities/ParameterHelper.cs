using System;
using System.Reflection;

namespace ChatBeet.Utilities
{
    public static class ParameterHelper
    {
        /// <summary>
        /// If a character is in the first string, move that segment to the second string
        /// </summary>
        /// <param name="pair">Pair of strings to check</param>
        /// <param name="divider">Delimiter character</param>
        public static (string, string) ForceCharacterOnRight((string, string) pair, char divider)
        {
            if (pair.Item1.Contains(divider))
            {
                var index = pair.Item1.IndexOf(divider);
                var start = pair.Item1.Substring(0, index);
                var end = string.Concat(pair.Item1.Substring(index + 1, pair.Item1.Length - index - 1), divider, pair.Item2);
                return (start, end);
            }
            return pair;
        }

        public static string GetFriendlyTypeName(this ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                return $"{parameterType.GetGenericArguments()[0].Name}?";
            return parameterType.Name;
        }
    }
}
