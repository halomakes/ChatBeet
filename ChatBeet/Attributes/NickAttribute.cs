using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Attributes
{
    public class NickAttribute : RegularExpressionAttribute
    {
        public NickAttribute(bool allowCaret = false) : base(allowCaret ? @"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+|\^" : @"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+")
        { }

        public override string FormatErrorMessage(string name) => $"The {name} field must be a valid nick.";
    }
}
