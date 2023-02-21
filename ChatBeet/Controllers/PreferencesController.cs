using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
public class PreferencesController : Controller
{
    private readonly UserPreferencesService _prefsService;
    private readonly WebIdentityService _webIdentity;

    public PreferencesController(UserPreferencesService prefsService, WebIdentityService webIdentity)
    {
        _prefsService = prefsService;
        _webIdentity = webIdentity;
    }

    private async Task<User> GetCurrentUserAsync() => await _webIdentity.GetCurrentUserAsync();

    /// <summary>
    /// Get all set preferences
    /// </summary>
    [HttpGet, Authorize]
    public async Task<IEnumerable<UserPreferenceSetting>> GetPreferences() => await _prefsService.Get((await GetCurrentUserAsync()).Id);

    /// <summary>
    /// Get an individual preference
    /// </summary>
    /// <param name="preference">Preference to get</param>
    [HttpGet("{preference}"), Authorize]
    public async Task<string> GetPreference([FromRoute] UserPreference preference) => await _prefsService.Get((await GetCurrentUserAsync()).Id, preference);

    /// <summary>
    /// Set a preference
    /// </summary>
    /// <param name="change">Values to set</param>
    [HttpPut, Authorize]
    public async Task<ActionResult<string>> SetPreference([FromBody] PreferenceChangeRequest change)
    {
        var validationMessage = _prefsService.GetValidation(change.Preference, change.Value);
        if (!string.IsNullOrEmpty(validationMessage))
            return BadRequest(validationMessage);
        var populatedChange = new PreferenceChange
        {
            User = await GetCurrentUserAsync(),
            Preference = change.Preference,
            Value = change.Value
        };
        var normalized = await _prefsService.Set(populatedChange);
        return Json(normalized);
    }
}