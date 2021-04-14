using ChatBeet.Data;
using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FixedTimeRangesController : ControllerBase
    {
        private readonly ProgressContext dbContext;

        public FixedTimeRangesController(ProgressContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/FixedTimeRanges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FixedTimeRange>>> GetFixedTimeRanges()
        {
            return await dbContext.FixedTimeRanges.AsQueryable().ToListAsync();
        }

        // GET: api/FixedTimeRanges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FixedTimeRange>> GetFixedTimeRange(string id)
        {
            var fixedTimeRange = await dbContext.FixedTimeRanges.FindAsync(id);

            if (fixedTimeRange == null)
            {
                return NotFound();
            }

            return fixedTimeRange;
        }

        // PUT: api/FixedTimeRanges/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutFixedTimeRange(string id, FixedTimeRange fixedTimeRange)
        {
            if (id != fixedTimeRange.Key)
            {
                return BadRequest();
            }

            dbContext.Entry(fixedTimeRange).State = EntityState.Modified;

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixedTimeRangeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FixedTimeRanges
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        public async Task<ActionResult<FixedTimeRange>> PostFixedTimeRange(FixedTimeRange fixedTimeRange)
        {
            dbContext.FixedTimeRanges.Add(fixedTimeRange);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FixedTimeRangeExists(fixedTimeRange.Key))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFixedTimeRange", new { id = fixedTimeRange.Key }, fixedTimeRange);
        }

        // DELETE: api/FixedTimeRanges/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteFixedTimeRange(string id)
        {
            var fixedTimeRange = await dbContext.FixedTimeRanges.FindAsync(id);
            if (fixedTimeRange == null)
            {
                return NotFound();
            }

            dbContext.FixedTimeRanges.Remove(fixedTimeRange);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool FixedTimeRangeExists(string id)
        {
            return dbContext.FixedTimeRanges.Any(e => e.Key == id);
        }
    }
}
