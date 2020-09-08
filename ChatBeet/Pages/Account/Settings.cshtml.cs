using ChatBeet.Annotations;
using ChatBeet.Services;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Pages.Account
{
    [Authorize]
    public class SettingsModel : PageModel
    {
        private readonly UserPreferencesService userPreferences;
        private readonly BooruService booru;

        public string Nick { get; private set; }
        public Dictionary<string, string> Settings { get; private set; }
        public IEnumerable<string> BlacklistedTags { get; private set; }
        public IEnumerable<string> GlobalTags { get; private set; }

        public SettingsModel(UserPreferencesService userPreferences, BooruService booru)
        {
            this.userPreferences = userPreferences;
            this.booru = booru;
        }

        public async Task OnGet()
        {
            Nick = User.Claims.FirstOrDefault(c => c.Type == "nick")?.Value;
            Settings = (await userPreferences.Get(Nick)).ToDictionary(s => s.Preference.GetAttribute<ParameterAttribute>().DisplayName, s => s.Value);
            BlacklistedTags = await booru.GetBlacklistedTags(Nick);
            GlobalTags = booru.GetGlobalBlacklistedTags();
        }
    }
}
