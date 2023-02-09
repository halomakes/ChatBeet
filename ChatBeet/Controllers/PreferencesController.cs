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
    private readonly UserPreferencesService prefsService;

    public PreferencesController(UserPreferencesService prefsService)
    {
        this.prefsService = prefsService;
    }

    /// <summary>
    /// Get all set preferences
    /// </summary>
    [HttpGet, Authorize]
    public async Task<IEnumerable<UserPreferenceSetting>> GetPreferences() => await prefsService.Get(User?.Identity?.Name);

    /// <summary>
    /// Get an individual preference
    /// </summary>
    /// <param name="preference">Preference to get</param>
    [HttpGet("{preference}"), Authorize]
    public async Task<string> GetPreference([FromRoute] UserPreference preference) => await prefsService.Get(User?.Identity?.Name, preference);

    /// <summary>
    /// Set a preference
    /// </summary>
    /// <param name="change">Values to set</param>
    [HttpPut, Authorize]
    public async Task<ActionResult<string>> SetPreference([FromBody] PreferenceChange change)
    {
        change.Nick = User?.Identity?.Name;
        if (string.IsNullOrEmpty(change.Nick))
            return Unauthorized("You do not have access to user preferences.");

        var normalized = await prefsService.Set(change);

        return Ok(normalized);
    }
}