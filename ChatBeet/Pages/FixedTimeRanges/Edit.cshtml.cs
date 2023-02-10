using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ChatBeet.Data;

namespace ChatBeet.Pages.FixedTimeRanges;

[Authorize]
public class EditModel : PageModel
{
    private readonly IProgressRepository _context;

    public EditModel(IProgressRepository context)
    {
        _context = context;
    }

    [BindProperty]
    public ProgressSpan ProgressSpan { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        ProgressSpan = await _context.Spans.AsQueryable().FirstOrDefaultAsync(m => m.Key == id);

        if (ProgressSpan == null)
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

        _context.Attach(ProgressSpan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FixedTimeRangeExists(ProgressSpan.Key))
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
        return _context.Spans.Any(e => e.Key == id);
    }
}