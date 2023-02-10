using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
public class DefinitionsController : ControllerBase
{
    private readonly MemoryCellContext _db;
    private readonly IMediator _messageQueue;

    public DefinitionsController(MemoryCellContext db, IMediator messageQueue)
    {
        _db = db;
        _messageQueue = messageQueue;
    }

    private async Task SetDefinition(string key, string value)
    {
        var info = new DefinitionChange();

        info.Key = key.Trim();
        info.NewValue = value.Trim();
        var oldDef = await _db.MemoryCells.AsQueryable().FirstOrDefaultAsync(m => m.Key.ToLower() == key.ToLower());
        info.NewNick = User.GetNick();
        if (oldDef == default)
        {
            _db.MemoryCells.Add(new MemoryCell
            {
                Key = info.Key,
                Value = info.NewValue,
                Author = info.NewNick
            });
        }
        else
        {
            info.OldValue = oldDef.Value;
            info.OldNick = oldDef.Author;

            oldDef.Author = info.NewNick;
            oldDef.Value = info.NewValue;
        }

        await _db.SaveChangesAsync();
        await _messageQueue.Publish(info);
    }


    /// <summary>
    /// Get all definitions
    /// </summary>
    [HttpGet]
    public IQueryable<MemoryCell> GetDefinitions() => _db.MemoryCells;

    /// <summary>
    /// Get a definition
    /// </summary>
    /// <param name="key">Ke yof definition</param>
    [HttpGet("{key}")]
    public async Task<ActionResult<MemoryCell>> GetDefinition([FromRoute] string key)
    {
        var def = await _db.MemoryCells
            .AsQueryable()
            .FirstOrDefaultAsync(m => m.Key.ToLower() == key.ToLower());
        if (def == null)
            return NotFound();
        else return Ok(def);
    }

    /// <summary>
    /// Replace an existing definition
    /// </summary>
    /// <param name="key">Key of definition</param>
    /// <param name="cell">Definition to set</param>
    [Authorize]
    [HttpPut("{key}")]
    public async Task<ActionResult> UpdateDefinition([FromRoute] string key, [FromBody] MemoryCell cell)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (key == default)
            return BadRequest("Key is required.");
        if (key != cell.Key)
            return BadRequest("Key in path and body must match.");
        if (!await _db.MemoryCells.AsNoTracking().AnyAsync(s => s.Key.ToLower() == key.ToLower()))
            return BadRequest($"Definition for {key} does not exist.");

        await SetDefinition(key, cell.Value);

        return CreatedAtAction(nameof(GetDefinition), new { key });
    }

    /// <summary>
    /// Add a new definition
    /// </summary>
    /// <param name="cell">Definition to set</param>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddDefinition([FromBody] MemoryCell cell)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await SetDefinition(cell.Key, cell.Value);

        return CreatedAtAction(nameof(GetDefinition), new { cell.Key });
    }
}