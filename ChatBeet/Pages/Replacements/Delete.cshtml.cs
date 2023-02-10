using ChatBeet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Replacements;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly ReplacementContext _db;

    public DeleteModel(ReplacementContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> OnGet([FromRoute] int id, [FromRoute] string input)
    {
        var map = await _db.Mappings.AsQueryable().FirstOrDefaultAsync(m => m.SetId == id && m.Input == input);
        if (map != default)
        {
            _db.Mappings.Remove(map);
            await _db.SaveChangesAsync();
        }
        return RedirectToPage("Index", new { id, input });
    }
}