using ChatBeet.Data;
using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.FixedTimeRanges;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ProgressContext _context;

    public DeleteModel(ProgressContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        FixedTimeRange = await _context.FixedTimeRanges.FindAsync(id);

        if (FixedTimeRange != null)
        {
            _context.FixedTimeRanges.Remove(FixedTimeRange);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}