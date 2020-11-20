using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ChatBeet.Attributes
{
    public class RegularExpressionWithOptionsAttribute : RegularExpressionAttribute
    {
        public RegularExpressionWithOptionsAttribute(string pattern) : base(pattern) { }

        public RegexOptions RegexOptions { get; set; }

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value as string))
                return true;

            return Regex.IsMatch(value as string, "^" + Pattern + "$", RegexOptions);
        }
    }
}
