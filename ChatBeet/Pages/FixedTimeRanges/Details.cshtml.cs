using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Pages.FixedTimeRanges;

public class DetailsModel : PageModel
{
    private readonly IProgressRepository _context;

    public DetailsModel(IProgressRepository context)
    {
        _context = context;
    }

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
}