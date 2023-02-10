using ChatBeet.Services;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BlacklistController : ControllerBase
{
    private readonly BooruService _booru;

    public BlacklistController(BooruService booru)
    {
        _booru = booru;
    }

    /// <summary>
    /// Get tags currently on your blacklist
    /// </summary>
    [HttpGet, Authorize]
    public async Task<ActionResult<IEnumerable<string>>> Get()
    {
        return Ok(await _booru.GetBlacklistedTags(User?.Identity?.Name));
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
        await _booru.WhitelistTags(User.GetNick(), allTags);
    }

    /// <summary>
    /// Add tags to your booru blacklist
    /// </summary>
    /// <param name="tagList">String-delimited tags</param>
    [HttpPatch("{tagList}")]
    public async Task Add(string tagList)
    {
        var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        await _booru.BlacklistTags(User.GetNick(), allTags);
    }
}