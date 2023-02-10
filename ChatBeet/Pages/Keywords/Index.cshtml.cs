using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Keywords;

public class IndexModel : PageModel
{
    private readonly KeywordService _service;

    public IEnumerable<KeywordStat> Stats { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public IndexModel(KeywordService service)
    {
        _service = service;
    }

    public async Task OnGet()
    {
        Stats = await _service.GetKeywordStatsAsync();
        LastUpdated = KeywordService.StatsLastUpdated;
    }
}