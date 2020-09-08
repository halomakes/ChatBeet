using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Models
{
    public class LoginTokenRequest
    {
        [RegularExpression(@"^[a-z0-9_\-\[\]\\^{}|`]{2,16}$")]
        public string Nick { get; set; }
        public string AuthToken { get; set; }
    }
}
