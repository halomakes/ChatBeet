using System.Threading.Tasks;
using ChatBeet.Authorization;
using ChatBeet.Notifications;
using ChatBeet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

[Route("api/Guilds/{guildId}/[controller]")]
[ApiController]
[Authorize(Policy = InGuildRequirement.Policy)]
public class HighGroundController : Controller
{
    private readonly MustafarService _mustafar;

    public HighGroundController(MustafarService mustafar)
    {
        _mustafar = mustafar;
    }

    [HttpGet]
    public async Task<ActionResult<HighGroundChangeNotification>> GetHighGround([FromRoute] ulong guildId) => Ok(await _mustafar.GetChangeAsync(guildId));
}