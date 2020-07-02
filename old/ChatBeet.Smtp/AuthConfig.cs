using System.Collections.Generic;

namespace ChatBeet.Smtp
{
    public class AuthConfig
    {
        public bool AllowAny { get; set; }
        public IEnumerable<CredentialSet> AuthorizedUsers { get; set; }
    }
}