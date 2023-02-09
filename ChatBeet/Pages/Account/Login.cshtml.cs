using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace ChatBeet.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty] public string ReturnUrl { get; set; }

    public IActionResult OnGet(string ReturnUrl = default, [FromQuery(Name = "n")] string nick = default)
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(ReturnUrl ?? "/Account/Success");
        }

        this.ReturnUrl = ReturnUrl;
        return Challenge(new AuthenticationProperties { RedirectUri = ReturnUrl ?? "/" }, "Discord");
    }
}