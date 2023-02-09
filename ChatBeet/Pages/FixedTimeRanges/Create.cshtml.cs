using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ChatBeet.Pages.FixedTimeRanges;

[Authorize]
public class CreateModel : PageModel
{
    private readonly Data.ProgressContext _context;

    public CreateModel(Data.ProgressContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public FixedTimeRange FixedTimeRange { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.FixedTimeRanges.Add(FixedTimeRange);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}