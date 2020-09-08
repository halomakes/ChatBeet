using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Models
{
    public class LoginTokenRequest
    {
        [RegularExpression(@"^[a-z0-9_\-\[\]\\^{}|`]{2,16}$")]
        public string Nick { get; set; }

        [Display(Name = "Authentication Token")]
        public string AuthToken { get; set; }
    }
}
