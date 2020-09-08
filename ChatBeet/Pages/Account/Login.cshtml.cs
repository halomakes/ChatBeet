using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ChatBeet.Models;
using ChatBeet.Services;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatBeet.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MessageQueueService messageQueue;
        private readonly UserManager<IdentityUser> userManager;
        private static readonly string TokenAction = "passwordless-auth";
        private static readonly string TokenProvider = "Default";

        public LoginModel(MessageQueueService messageQueue, UserManager<IdentityUser> userManager)
        {
            this.messageQueue = messageQueue;
            this.userManager = userManager;
        }

        [BindProperty]
        public LoginTokenRequest LoginInfo { get; set; }

        public bool IsOnVerificationStep { get; set; }

        public void OnGet()
        {
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoginInfo = null;
                return Page();
            }

            if (string.IsNullOrEmpty(LoginInfo.AuthToken))
            {
                // send token
                var user = await userManager.FindByNameAsync(LoginInfo.Nick);
                if (user == default)
                {
                    user = new IdentityUser(LoginInfo.Nick);
                    await userManager.CreateAsync(user);
                }

                var token = await userManager.GenerateUserTokenAsync(user, TokenProvider, TokenAction);

                messageQueue.Push(new LoginTokenRequest { Nick = LoginInfo.Nick, AuthToken = token });
            }
            else
            {
                // handle login
            }

            return Page();
        }
    }
}
