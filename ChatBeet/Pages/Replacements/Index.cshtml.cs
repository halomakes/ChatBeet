using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Replacements
{
    [ResponseCache(Duration = 300)]
    public class IndexModel : PageModel
    {
        private readonly ReplacementContext db;
        public IEnumerable<Stat<ReplacementSet>> Stats { get; set; }

        public IndexModel(ReplacementContext db)
        {
            this.db = db;
        }

        public async Task OnGet()
        {
            Stats = await db.Sets.AsQueryable()
                .Include(s => s.Mappings)
                .Select(s => new Stat<ReplacementSet> { Item = s, Count = s.Mappings.Count() })
                .ToListAsync();
        }
    }
}
