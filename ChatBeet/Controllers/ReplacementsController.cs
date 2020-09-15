using ChatBeet.Data;
using ChatBeet.Data.Entities;
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
            else return set;
        }

        [HttpGet("{id}/Mappings")]
        public IQueryable<ReplacementMapping> GetSetMappings([FromRoute] int id) => db.Mappings.AsQueryable().Where(s => s.SetId == id);
    }
}
