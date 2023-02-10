using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace ChatBeet.Pages.Account;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Account/Logout");
        }

        return Page();
    }
}