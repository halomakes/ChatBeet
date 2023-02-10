using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Pages.FixedTimeRanges;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IProgressRepository _context;

    public DeleteModel(IProgressRepository context)
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

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        ProgressSpan = await _context.Spans.FirstOrDefaultAsync(s => s.Key == id);

        if (ProgressSpan != null)
        {
            _context.Spans.Remove(ProgressSpan);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}