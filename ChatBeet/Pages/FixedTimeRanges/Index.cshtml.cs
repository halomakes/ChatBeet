using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.FixedTimeRanges
{
    public class IndexModel : PageModel
    {
        private readonly ChatBeet.Data.ProgressContext _context;

        public IndexModel(ChatBeet.Data.ProgressContext context)
        {
            _context = context;
        }

        public IList<FixedTimeRange> FixedTimeRange { get; set; }

        public async Task OnGetAsync()
        {
            FixedTimeRange = await _context.FixedTimeRanges.AsQueryable().ToListAsync();
        }
    }
}
