using ChatBeet.Services;
using ChatBeet.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpDelete("{tagList}")]
        public async Task Remove(string tagList)
        {
            var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            await booru.WhitelistTags(User.GetNick(), allTags);
        }

        [HttpPatch("{tagList}")]
        public async Task Add(string tagList)
        {
            var allTags = tagList.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            await booru.BlacklistTags(User.GetNick(), allTags);
        }
    }
}
