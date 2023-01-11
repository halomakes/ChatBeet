using ChatBeet.Models;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Web;

namespace ChatBeet.Rules
{
    public class LoginTokenRule : IMessageRule<LoginTokenRequest>
    {
        private readonly string _canonicalUrl;

        public LoginTokenRule(IConfiguration configuration)
        {
            _canonicalUrl = configuration.GetValue<string>("CanonicalUrl");
        }

        public IEnumerable<IClientMessage> Respond(LoginTokenRequest incomingMessage)
        {
            if (incomingMessage.Mode == LoginTokenRequest.LoginMode.Link)
            {
                yield return new PrivateMessage(incomingMessage.Nick, $"Use this link to log in: {IrcValues.AQUA}{_canonicalUrl}/Account/Login?n={HttpUtility.UrlEncode(incomingMessage.Nick)}#{incomingMessage.AuthToken}{IrcValues.RESET}");
            }
            else
            {
                yield return new PrivateMessage(incomingMessage.Nick, $"Your login token is {IrcValues.AQUA}{incomingMessage.AuthToken}{IrcValues.RESET}");
            }
        }
    }
}
