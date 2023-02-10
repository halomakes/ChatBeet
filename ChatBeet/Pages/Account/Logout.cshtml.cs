using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Account;

public class LogoutModel : PageModel
{
    public Task<IActionResult> OnGet()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            // await logonService.LogoutAsync();
            return Task.FromResult<IActionResult>(RedirectToPage("/Account/Logout"));
        }

        return Task.FromResult<IActionResult>(Page());
    }
}