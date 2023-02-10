using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using ChatBeet.Data;

namespace ChatBeet.Pages.FixedTimeRanges;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IProgressRepository _context;

    public CreateModel(Data.IProgressRepository context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public ProgressSpan ProgressSpan { get; set; }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Spans.Add(ProgressSpan);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}