using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

public class LoginController : Controller
{
    [HttpPost("Discord")]
    public Task<IActionResult> DiscordLogin([FromForm] string returnUrl)
    {
        return Task.FromResult<IActionResult>(Challenge(new AuthenticationProperties { RedirectUri = returnUrl ?? "/" }, "Discord"));
    }
}