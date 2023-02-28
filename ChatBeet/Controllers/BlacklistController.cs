using ChatBeet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BlacklistController : ControllerBase
{
    private readonly BooruService _booru;
    private readonly WebIdentityService _webIdentity;

    public BlacklistController(BooruService booru, WebIdentityService webIdentity)
    {
        _booru = booru;
        _webIdentity = webIdentity;
    }

    /// <summary>
    /// Get tags currently on your blacklist
    /// </summary>
    [HttpGet, Authorize]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        return Ok(await _booru.GetBlacklistedTags((await _webIdentity.GetCurrentUserAsync()).Id));
    }

    /// <summary>
    /// Get globally blacklisted tags
    /// </summary>
    [HttpGet("global")]
    public ActionResult<IEnumerable<string>> GetGlobal() => Ok(_booru.GetGlobalBlacklistedTags());

    /// <summary>
    /// Remove tags from your booru blacklist
    /// </summary>
    /// <param name="tagList">String-delimited tags</param>
    [HttpDelete("{tagList}")]
    public async Task Remove(string tagList)
    {
        var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        await _booru.WhitelistTags((await _webIdentity.GetCurrentUserAsync()).Id, allTags);
    }

    /// <summary>
    /// Add tags to your booru blacklist
    /// </summary>
    /// <param name="tagList">String-delimited tags</param>
    [HttpPatch("{tagList}")]
    public async Task Add(string tagList)
    {
        var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        await _booru.BlacklistTags((await _webIdentity.GetCurrentUserAsync()).Id, allTags);
    }
}