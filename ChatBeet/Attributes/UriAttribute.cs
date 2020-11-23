using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Attributes
{
    public class UriAttribute : RegularExpressionAttribute
    {
        public UriAttribute() : base(@"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)") { }

        public override string FormatErrorMessage(string name) => $"The {name} field must be a valid URI.";
    }
}
