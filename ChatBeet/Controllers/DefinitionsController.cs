using ChatBeet.Data.Entities;
using ChatBeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Controllers;

[Route("api/Guilds/{guildId}/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
public class DefinitionsController : ControllerBase
{
    private readonly IDefinitionsRepository _db;
    private readonly IMediator _messageQueue;
    private readonly WebIdentityService _identity;

    public DefinitionsController(IDefinitionsRepository db, IMediator messageQueue, WebIdentityService identity)
    {
        _db = db;
        _messageQueue = messageQueue;
        _identity = identity;
    }

    private async Task SetDefinition(ulong guildId, string key, string value)
    {
        var oldDef = await _db.Definitions
            .Include(d => d.Author)
            .FirstOrDefaultAsync(m => m.Key.ToLower() == key.ToLower());
        var info = new DefinitionChange
        {
            Key = key.Trim(),
            GuildId = guildId,
            NewValue = value.Trim(),
            NewUser = (await _identity.GetCurrentUserAsync())!
        };
        if (oldDef == default)
        {
            _db.Definitions.Add(new Definition
            {
                Key = info.Key,
                Value = info.NewValue,
                CreatedBy = info.NewUser.Id
            });
        }
        else
        {
            info.OldValue = oldDef.Value;
            info.OldUser = oldDef.Author;

            oldDef.CreatedBy = info.NewUser.Id;
            oldDef.Value = info.NewValue;
        }

        await _db.SaveChangesAsync();
        await _messageQueue.Publish(info);
    }


    /// <summary>
    /// Get all definitions
    /// </summary>
    [HttpGet]
    public IQueryable<Definition> GetDefinitions([FromRoute] ulong guildId) => _db.Definitions
        .Where(d => d.GuildId == guildId);

    /// <summary>
    /// Get a definition
    /// </summary>
    /// <param name="guildId">ID of guild to set definition in</param>
    /// <param name="key">Key of definition</param>
    [HttpGet("{key}")]
    public async Task<ActionResult<Definition>> GetDefinition([FromRoute] ulong guildId, [FromRoute] string key)
    {
        var def = await _db.Definitions
            .Where(d => d.GuildId == guildId)
            .FirstOrDefaultAsync(m => m.Key.ToLower() == key.ToLower());
        if (def == null)
            return NotFound();
        return Ok(def);
    }

    /// <summary>
    /// Replace an existing definition
    /// </summary>
    /// <param name="guildId">ID of guild to set definition in</param>
    /// <param name="key">Key of definition</param>
    /// <param name="cell">Definition to set</param>
    [Authorize]
    [HttpPut("{key}")]
    public async Task<ActionResult> UpdateDefinition([FromRoute] ulong guildId, [FromRoute] string? key, [FromBody] Definition cell)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (key == default)
            return BadRequest("Key is required.");
        if (guildId != cell.GuildId)
            return BadRequest("Guild ID in path and body must match.");
        if (key != cell.Key)
            return BadRequest("Key in path and body must match.");
        if (!await _db.Definitions.AsNoTracking()
                .Where(d => d.GuildId == guildId)
                .AnyAsync(s => s.Key.ToLower() == key.ToLower()))
            return BadRequest($"Definition for {key} does not exist.");

        await SetDefinition(guildId, key, cell.Value!);

        return CreatedAtAction(nameof(GetDefinition), new { key });
    }

    /// <summary>
    /// Add a new definition
    /// </summary>
    /// <param name="guildId">ID of guild to set definition in</param>
    /// <param name="cell">Definition to set</param>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddDefinition([FromRoute] ulong guildId, [FromBody] Definition cell)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await SetDefinition(guildId, cell.Key, cell.Value!);

        return CreatedAtAction(nameof(GetDefinition), new { cell.Key });
    }
}