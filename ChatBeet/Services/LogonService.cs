using ChatBeet.Models;
using GravyBot;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class LogonService
    {
        private readonly MessageQueueService messageQueue;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private static readonly string TokenAction = "passwordless-auth";
        private static readonly string TokenProvider = "Default";

        public LogonService(MessageQueueService messageQueue, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.messageQueue = messageQueue;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task SendLoginTokenAsync(string nick)
        {
            var user = await GetUserAsync(nick);
            // send token
            if (user == default)
            {
                user = new IdentityUser(nick) { UserName = nick };
                await userManager.CreateAsync(user);
            }

            var token = await userManager.GenerateUserTokenAsync(user, TokenProvider, TokenAction);

            messageQueue.Push(new LoginTokenRequest { Nick = nick, AuthToken = token });
        }

        public Task<IdentityUser> GetUserAsync(string nick) => userManager.FindByNameAsync(nick.Trim().ToLower());

        public async Task ValidateTokenAsync(string nick, string token, bool persist)
        {
            var user = await GetUserAsync(nick);
            // handle login
            if (user == default)
                throw new UserNotFoundException();

            var isValid = await userManager.VerifyUserTokenAsync(user, TokenProvider, TokenAction, token);
            if (isValid)
            {
                await userManager.UpdateSecurityStampAsync(user);
                var claims = new ClaimsIdentity(new List<Claim> {
                        new Claim("sub", user.Id),
                        new Claim("nick", user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.UserName),
                        new Claim(ClaimTypes.Name, user.UserName)
                    }, IdentityConstants.ApplicationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = persist
                };
                await httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(claims), authProperties);
                messageQueue.Push(new LoginCompleteNotification { Nick = user.UserName });
                return;
            }
            else
            {
                throw new InvalidTokenException();
            }
        }

        public Task LogoutAsync() => httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        public class UserNotFoundException : Exception { }

        public class InvalidTokenException : Exception { }
    }
}
