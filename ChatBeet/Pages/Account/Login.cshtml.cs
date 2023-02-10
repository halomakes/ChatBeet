using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace ChatBeet.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty] public string ReturnUrl { get; set; }

    public IActionResult OnGet(string returnUrl = default, [FromQuery(Name = "n")] string nick = default)
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(returnUrl ?? "/Account/Success");
        }

        ReturnUrl = returnUrl;
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, "Discord");
    }
}