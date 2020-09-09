using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Models
{
    public class LoginTokenRequest
    {
        [RegularExpression(@"^[A-z0-9_\-\[\]\\^{}|`]{2,16}$", ErrorMessage = "Enter a valid IRC nick.")]
        public string Nick { get; set; }

        [Display(Name = "Authentication Token")]
        public string AuthToken { get; set; }
    }
}
