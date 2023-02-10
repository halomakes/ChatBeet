using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Pages;

public class CrewmatesModel : PageModel
{
    private readonly SuspicionService _suspicionService;

    public CrewmatesModel( SuspicionService suspicionService)
    {
        _suspicionService = suspicionService;
    }

    public List<SuspicionRank> Ranks { get; private set; }

    public async Task OnGet()
    {
        Ranks = (await _suspicionService.GetSuspicionLevels()).ToList();
    }
}