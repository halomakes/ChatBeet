using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(Duration = 300)]
    public class KeywordsController : ControllerBase
    {
        private readonly KeywordService service;

        public KeywordsController(KeywordService service)
        {
            this.service = service;
        }

        [HttpGet]
        public Task<IEnumerable<KeywordStat>> GetStats() => service.GetKeywordStatsAsync();

        [HttpGet("{id}")]
        public Task<KeywordStat> GetStat([FromRoute] int id) => service.GetKeywordStatAsync(id);
    }
}
