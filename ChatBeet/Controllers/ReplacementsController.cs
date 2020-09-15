using ChatBeet.Data;
using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(Duration = 300)]
    public class ReplacementsController : ControllerBase
    {
        private readonly ReplacementContext db;

        public ReplacementsController(ReplacementContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IQueryable<ReplacementSet> GetSets() => db.Sets;

        [HttpGet("{id}")]
        public async Task<ActionResult<ReplacementSet>> GetSet([FromRoute] int id)
        {
            var set = await db.Sets
                .Include(s => s.Mappings)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (set == null)
                return NotFound();
            else return Ok(set);
        }

        [HttpGet("{id}/Mappings")]
        public IQueryable<ReplacementMapping> GetSetMappings([FromRoute] int id) => db.Mappings.AsQueryable().Where(s => s.SetId == id);

        [HttpGet("{id}/Mappings/{input}")]
        public async Task<ActionResult<ReplacementMapping>> GetMapping([FromRoute] int id, [FromRoute] string input)
        {
            var map = await db.Mappings
                .AsQueryable()
                .Where(m => m.SetId == id && m.Input.ToLower() == input.ToLower())
                .FirstOrDefaultAsync();
            if (map == default)
                return NotFound();
            else
                return Ok(map);
        }

        [Authorize]
        [HttpPost("{id}/Mappings")]
        public async Task<ActionResult> AddMapping([FromRoute] int id, [FromBody] ReplacementMapping mapping)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id == default)
                return BadRequest("Set ID is required.");
            mapping.SetId = id;
            if (!await db.Sets.AsQueryable().AnyAsync(s => s.Id == id))
                return BadRequest($"Set {id} does not exist.");
            if (await db.Mappings.AsQueryable().AnyAsync(m => m.SetId == id && m.Input.ToLower() == mapping.Input.ToLower()))
                return BadRequest($"Set {id} already contains key {mapping.Input}");

            db.Mappings.Add(mapping);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMapping), new { id, input = mapping.Input });
        }
    }
}
