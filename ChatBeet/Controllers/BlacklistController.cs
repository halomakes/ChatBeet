using ChatBeet.Services;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BlacklistController : ControllerBase
    {
        private readonly BooruService booru;

        public BlacklistController(BooruService booru)
        {
            this.booru = booru;
        }

        /// <summary>
        /// Get tags currently on your blacklist
        /// </summary>
        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return Ok(await booru.GetBlacklistedTags(User?.Identity?.Name));
        }

        /// <summary>
        /// Get globally blacklisted tags
        /// </summary>
        [HttpGet("global")]
        public ActionResult<IEnumerable<string>> GetGlobal() => Ok(booru.GetGlobalBlacklistedTags());

        /// <summary>
        /// Remove tags from your booru blacklist
        /// </summary>
        /// <param name="tagList">String-delimited tags</param>
        [HttpDelete("{tagList}")]
        public async Task Remove(string tagList)
        {
            var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            await booru.WhitelistTags(User.GetNick(), allTags);
        }

        /// <summary>
        /// Add tags to your booru blacklist
        /// </summary>
        /// <param name="tagList">String-delimited tags</param>
        [HttpPatch("{tagList}")]
        public async Task Add(string tagList)
        {
            var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            await booru.BlacklistTags(User.GetNick(), allTags);
        }
    }
}
