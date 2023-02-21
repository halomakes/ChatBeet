using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ChatBeet.Authorization;
using ChatBeet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

[Route("api/Guilds/{guildId}/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
[Authorize(Policy = InGuildRequirement.Policy)]
public class KarmaController : Controller
{
    private readonly KarmaService _karma;

    public KarmaController(KarmaService karma)
    {
        _karma = karma;
    }

    [HttpGet]
    public async Task<ActionResult<Dictionary<string, int>>> GetKarmaValues([FromRoute] ulong guildId) => Ok(await _karma.GetLevelsAsync(guildId));

    [HttpGet("{key}")]
    public async Task<ActionResult<int>> GetKarmaValue([FromRoute] ulong guildId, [FromRoute, Required] string key) => Json(await _karma.GetLevelAsync(guildId, key));
}