using SmtpServer;
using SmtpServer.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Smtp
{
    public class BasicUserAuthenticator : IUserAuthenticator, IUserAuthenticatorFactory
    {
        private readonly AuthConfig config;

        public BasicUserAuthenticator(AuthConfig config)
        {
            this.config = config;
        }

        public Task<bool> AuthenticateAsync(ISessionContext context, string user, string password, CancellationToken cancellationToken) =>
            Task.FromResult(IsAuthorized(user, password));

        public IUserAuthenticator CreateInstance(ISessionContext context)
        {
            return new BasicUserAuthenticator(config);
        }

        private bool IsAuthorized(string user, string password)
        {
            if (config.AllowAny)
                return true;
            return config.AuthorizedUsers.Any(u => u.Username.ToLower() == user.ToLower() && u.Password == password);
        }
    }
}
