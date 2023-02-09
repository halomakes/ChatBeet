using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Replacements;

[ResponseCache(Duration = 300)]
[Authorize]
public class IndexModel : PageModel
{
    private readonly ReplacementContext db;
    public IEnumerable<Stat<ReplacementSet>> Stats { get; set; }
    public int? LastUpdatedSetId { get; private set; }

    [BindProperty]
    public ReplacementMapping Info { get; set; }

    public IndexModel(ReplacementContext db)
    {
        this.db = db;
    }

    public async Task OnGetAsync()
    {
        await LoadInfo();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadInfo();
        LastUpdatedSetId = Info?.SetId;

        if (!ModelState.IsValid)
            return Page();
        if (!await db.Sets.AsQueryable().AnyAsync(s => s.Id == Info.SetId))
            ModelState.AddModelError("noSet", $"Set {Info.SetId} does not exist.");
        if (await db.Mappings.AsQueryable().AnyAsync(m => m.SetId == Info.SetId && m.Input.ToLower() == Info.Input.ToLower()))
            ModelState.AddModelError("mapTaken", $"Set already contains key {Info.Input}");
        if (!ModelState.IsValid)
            return Page();

        db.Mappings.Add(Info);
        await db.SaveChangesAsync();
        Info = null;

        return Page();
    }

    private async Task LoadInfo()
    {
        Stats = await db.Sets.AsQueryable()
            .Include(s => s.Mappings)
            .Select(s => new Stat<ReplacementSet> { Item = s, Count = s.Mappings.Count() })
            .ToListAsync();
    }
}