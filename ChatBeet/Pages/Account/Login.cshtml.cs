using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Account;

public class LoginModel : PageModel
{
    private readonly LogonService logonService;

    public LoginModel(LogonService logonService)
    {
        this.logonService = logonService;
    }

    [BindProperty]
    public LoginTokenRequest LoginInfo { get; set; }

    [BindProperty]
    public string ReturnUrl { get; set; }

    public string ValidationMessage { get; set; }

    public IActionResult OnGet(string ReturnUrl = default, [FromQuery(Name ="n")] string nick = default)
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(ReturnUrl ?? "/Account/Success");
        }

        this.ReturnUrl = ReturnUrl;

        if (!string.IsNullOrEmpty(nick))
        {
            LoginInfo ??= new();
            LoginInfo.Nick = nick;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            LoginInfo = null;
            return Page();
        }

        if (string.IsNullOrEmpty(LoginInfo.AuthToken))
        {
            await logonService.SendLoginTokenAsync(LoginInfo.Nick);
        }
        else
        {
            try
            {
                await logonService.ValidateTokenAsync(LoginInfo.Nick, LoginInfo.AuthToken, LoginInfo.Persist);
                return Redirect(string.IsNullOrEmpty(ReturnUrl) ? "/Account/Success" : ReturnUrl);
            }
            catch (LogonService.UserNotFoundException)
            {
                ValidationMessage = "User not found.  Try getting a fresh token.";
                LoginInfo = null;
                return Page();
            }
            catch (LogonService.InvalidTokenException)
            {
                ValidationMessage = "The provided authentication token was invalid.";
                LoginInfo = null;
                return Page();
            }
        }

        return Page();
    }
}