using ChatBeet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Replacements
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ReplacementContext db;

        public DeleteModel(ReplacementContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> OnGet([FromRoute] int id, [FromRoute] string input)
        {
            var map = await db.Mappings.AsQueryable().FirstOrDefaultAsync(m => m.SetId == id && m.Input == input);
            if (map != default)
            {
                db.Mappings.Remove(map);
                await db.SaveChangesAsync();
            }
            return RedirectToPage("Index", new { id, input });
        }
    }
}
