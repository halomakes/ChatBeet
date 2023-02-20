using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace ChatBeet.Controllers;

[Route("api/Guilds/{guildId}/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
[Authorize(Policy = InGuildRequirement.Policy)]
public class SuspicionsController : Controller
{
    private readonly SuspicionService _suspicionService;

    public SuspicionsController(SuspicionService suspicionService)
    {
        _suspicionService = suspicionService;
    }

    /// <summary>
    /// Get current suspicion levels
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SuspicionRank>>> GetSuspicionLevels([FromRoute]ulong guildId) => Ok(await _suspicionService.GetSuspicionLevels(guildId));
}