using ChatBeet.Data.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Pages.FixedTimeRanges;

public class IndexModel : PageModel
{
    private readonly IProgressRepository _context;

    public IndexModel(IProgressRepository context)
    {
        _context = context;
    }

    public IList<ProgressSpan> FixedTimeRange { get; set; }

    public async Task OnGetAsync()
    {
        FixedTimeRange = await _context.Spans.AsQueryable().ToListAsync();
    }
}