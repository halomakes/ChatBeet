using ChatBeet.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdministrationController : Controller
{
    private readonly DiscordBotConfiguration _config;

    public AdministrationController(IOptionsSnapshot<DiscordBotConfiguration> options)
    {
        _config = options.Value;
    }

    [HttpGet("Invite")]
    public ActionResult<Uri> GetInvitation() => Ok(_config.InvitationLink);
}