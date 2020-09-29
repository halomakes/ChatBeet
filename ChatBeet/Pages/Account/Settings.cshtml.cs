using ChatBeet.Attributes;
using ChatBeet.Models;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly MessageQueueService messageQueue;
        private readonly BooruService booru;

        public string Nick { get; private set; }
        public Dictionary<string, string> Settings { get; private set; }
        public IEnumerable<string> BlacklistedTags { get; private set; }
        public IEnumerable<string> GlobalTags { get; private set; }

        [BindProperty]
        public PreferenceChange Preference { get; set; }

        public SettingsModel(UserPreferencesService userPreferences, BooruService booru, MessageQueueService messageQueue)
        {
            this.userPreferences = userPreferences;
            this.booru = booru;
            this.messageQueue = messageQueue;
        }

        public async Task OnGet()
        {
            await PopulateValues();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var valMsg = userPreferences.GetValidation(Preference.Preference.Value, Preference.Value);
                if (string.IsNullOrEmpty(valMsg))
                {
                    Preference.Value = Preference.Value.Trim();
                    Preference.Nick = User.GetNick();
                    Preference.Value = await userPreferences.Set(Preference);
                    messageQueue.Push(Preference);
                    return RedirectToPage("/Account/Settings");
                }
                else
                {
                    ModelState.AddModelError("Pref", valMsg);
                }
            }

            await PopulateValues();
            return Page();
        }

        private async Task PopulateValues()
        {
            Nick = User.GetNick();
            Settings = (await userPreferences.Get(Nick)).ToDictionary(s => s.Preference.GetAttribute<ParameterAttribute>().DisplayName, s => s.Value);
            BlacklistedTags = await booru.GetBlacklistedTags(Nick);
            GlobalTags = booru.GetGlobalBlacklistedTags();
        }
    }
}
