using ChatBeet.Utilities;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Attributes
{
    public class UriAttribute : RegularExpressionAttribute
    {
        public UriAttribute() : base(RegexUtils.Uri) { }

        public override string FormatErrorMessage(string name) => $"The {name} field must be a valid URI.";
    }
}