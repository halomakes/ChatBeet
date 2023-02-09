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
public class EditModel : PageModel
{
    private readonly ProgressContext _context;

    public EditModel(ProgressContext context)
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

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(FixedTimeRange).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FixedTimeRangeExists(FixedTimeRange.Key))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool FixedTimeRangeExists(string id)
    {
        return _context.FixedTimeRanges.Any(e => e.Key == id);
    }
}