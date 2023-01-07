using ChatBeet.Services;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Commands.Irc
{
    public class LoginCommandProcessor : CommandProcessor
    {
        private readonly LogonService logonService;

        public LoginCommandProcessor(LogonService logonService)
        {
            this.logonService = logonService;
        }

        [Command("login", Description = "Log into the ChatBeet web UI.")]
        public async IAsyncEnumerable<IClientMessage> RequestLogin()
        {
            await logonService.SendLoginTokenAsync(IncomingMessage.From, mode: Models.LoginTokenRequest.LoginMode.Link);
            yield break;
        }
    }
}
