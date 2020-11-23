using ChatBeet.Utilities;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Attributes
{
    public class NickAttribute : RegularExpressionAttribute
    {
        public NickAttribute(bool allowCaret = false) : base(allowCaret ? @$"{RegexUtils.Nick}|\^" : RegexUtils.Nick)
        { }

        public override string FormatErrorMessage(string name) => $"The {name} field must be a valid nick.";
    }
}
