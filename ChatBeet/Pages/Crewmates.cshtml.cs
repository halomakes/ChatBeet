using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages
{
    public class CrewmatesModel : PageModel
    {
        private readonly UserPreferencesService userPreferencesService;
        private readonly SuspicionService suspicionService;

        public CrewmatesModel(UserPreferencesService userPreferencesService, SuspicionService suspicionService)
        {
            this.userPreferencesService = userPreferencesService;
            this.suspicionService = suspicionService;
        }

        public List<SuspicionRank> Ranks { get; private set; }

        public async Task OnGet()
        {
            var mostSuspicious = (await suspicionService.GetActiveSuspicionsAsync())
                .GroupBy(s => s.Suspect.ToLower())
                .Select(g => new { Nick = g.Key, Count = g.Count() })
                .OrderByDescending(t => t.Count)
                .Take(9)
                .ToList();

            var colorPrefs = await userPreferencesService.Get(mostSuspicious.Select(s => s.Nick), UserPreference.GearColor);

            Ranks = mostSuspicious.Select(s =>
            {
                var pref = colorPrefs.FirstOrDefault(p => p.Nick.Equals(s.Nick, StringComparison.OrdinalIgnoreCase));
                return new SuspicionRank
                {
                    Nick = pref?.Nick ?? s.Nick,
                    Level = s.Count,
                    Color = pref?.Value
                };
            }).ToList();
        }
    }
}
