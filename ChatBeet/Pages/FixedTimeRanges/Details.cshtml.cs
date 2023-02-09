using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.FixedTimeRanges;

public class DetailsModel : PageModel
{
    private readonly Data.ProgressContext _context;

    public DetailsModel(Data.ProgressContext context)
    {
        _context = context;
    }

    public FixedTimeRange FixedTimeRange { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        FixedTimeRange = await _context.FixedTimeRanges.AsQueryable().FirstOrDefaultAsync(m => m.Key == id);

        if (FixedTimeRange == null)
        {
            return NotFound();
        }
        return Page();
    }
}