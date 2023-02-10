using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;

namespace ChatBeet.Controllers;

[Route("api/Guilds/{guildId}/[controller]")]
[ApiController]
public class FixedTimeRangesController : ControllerBase
{
    private readonly IProgressRepository _dbContext;

    public FixedTimeRangesController(IProgressRepository dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get all time ranges
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProgressSpan>>> GetFixedTimeRanges([FromRoute] ulong guildId) =>
        await _dbContext.Spans
            .Where(s => s.GuildId == guildId)
            .ToListAsync();

    /// <summary>
    /// Get a time range
    /// </summary>
    /// <param name="guildId">ID of guild to search within</param>
    /// <param name="key">ID of the time range</param>
    [HttpGet("{key}")]
    public async Task<ActionResult<ProgressSpan>> GetFixedTimeRange([FromRoute] ulong guildId, [FromRoute] string key)
    {
        var fixedTimeRange = await _dbContext.Spans.FirstOrDefaultAsync(s => s.GuildId == guildId && s.Key == key);

        if (fixedTimeRange == null)
            return NotFound();

        return fixedTimeRange;
    }

    /// <summary>
    /// Update a time range
    /// </summary>
    /// <param name="guildId">ID of guild</param>
    /// <param name="id">ID of the time range</param>
    /// <param name="progressSpan">Values to set</param>
    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> PutFixedTimeRange([FromRoute] ulong guildId, [FromRoute] string id, [FromBody] ProgressSpan progressSpan)
    {
        if (id != progressSpan.Key || guildId != progressSpan.GuildId)
            return BadRequest();

        _dbContext.Entry(progressSpan).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FixedTimeRangeExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Create a time range
    /// </summary>
    /// <param name="guildId">ID of guild</param>
    /// <param name="progressSpan">Data to create</param>
    [HttpPost, Authorize]
    public async Task<ActionResult<ProgressSpan>> PostFixedTimeRange([FromRoute] ulong guildId, ProgressSpan progressSpan)
    {
        if (guildId != progressSpan.GuildId)
            return BadRequest();
        _dbContext.Spans.Add(progressSpan);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (FixedTimeRangeExists(progressSpan.Key))
                return Conflict();
            throw;
        }

        return CreatedAtAction("GetFixedTimeRange", new { id = progressSpan.Key }, progressSpan);
    }

    /// <summary>
    /// Delete a time range
    /// </summary>
    /// <param name="guildId">ID of guild</param>
    /// <param name="id">ID of time range</param>
    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> DeleteFixedTimeRange([FromRoute]ulong guildId, [FromRoute] string id)
    {
        var fixedTimeRange = await _dbContext.Spans.FirstOrDefaultAsync(s => s.GuildId == guildId && s.Key == id);
        if (fixedTimeRange == null)
        {
            return NotFound();
        }

        _dbContext.Spans.Remove(fixedTimeRange);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool FixedTimeRangeExists(string id)
    {
        return _dbContext.Spans.Any(e => e.Key == id);
    }
}