using ChatBeet.Annotations;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Account
{
    public class SettingsModel : PageModel
    {
        private readonly UserPreferencesService userPreferences;

        public string Nick { get; set; }
        public Dictionary<string, string> Settings { get; set; }

        public SettingsModel(UserPreferencesService userPreferences)
        {
            this.userPreferences = userPreferences;
        }

        [Authorize]
        public async Task OnGet()
        {
            Nick = User.Claims.FirstOrDefault(c => c.Type == "nick")?.Value;
            Settings = (await userPreferences.Get(Nick)).ToDictionary(s => s.Preference.GetAttribute<ParameterAttribute>().DisplayName, s => s.Value);
        }
    }
}
