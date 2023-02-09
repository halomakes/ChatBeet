using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly LogonService logonService;

    public LogoutModel(LogonService logonService)
    {
        this.logonService = logonService;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
            await logonService.LogoutAsync();
            return RedirectToPage("/Account/Logout");
        }

        return Page();
    }
}