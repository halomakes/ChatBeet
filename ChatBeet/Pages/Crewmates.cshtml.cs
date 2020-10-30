using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages
{
    public class CrewmatesModel : PageModel
    {
        private readonly UserPreferencesService userPreferencesService;
        private readonly SuspicionContext suspicionContext;

        public CrewmatesModel(UserPreferencesService userPreferencesService, SuspicionContext suspicionContext)
        {
            this.userPreferencesService = userPreferencesService;
            this.suspicionContext = suspicionContext;
        }

        public List<SuspicionRank> Ranks { get; private set; }

        public async Task OnGet()
        {
            var mostSuspicious = await suspicionContext.Suspicions
                .AsQueryable()
                .GroupBy(s => s.Suspect.ToLower())
                .Select(g => new { Nick = g.Key, Count = g.Count() })
                .OrderByDescending(t => t.Count)
                .Take(9)
                .ToListAsync();

            var colorPrefs = await userPreferencesService.Get(mostSuspicious.Select(s => s.Nick), UserPreference.GearColor);

            Ranks = mostSuspicious.Select(s => new SuspicionRank
            {
                Nick = s.Nick,
                Level = s.Count,
                Color = colorPrefs.Where(p => p.Nick == s.Nick).Select(p => p.Value).FirstOrDefault()
            }).ToList();
        }

        public struct SuspicionRank
        {
            public string Nick;
            public int Level;
            public string Color;
        }
    }
}
