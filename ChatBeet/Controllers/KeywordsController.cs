using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
public class KeywordsController : ControllerBase
{
    private readonly KeywordService _service;

    public KeywordsController(KeywordService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get stats for all keywords
    /// </summary>
    [HttpGet]
    public Task<IEnumerable<KeywordStat>> GetStats() => _service.GetKeywordStatsAsync();

    /// <summary>
    /// Get stats for a keyword
    /// </summary>
    /// <param name="id">ID of the keyword</param>
    [HttpGet("{id}")]
    public Task<KeywordStat> GetStat([FromRoute] Guid id) => _service.GetKeywordStatAsync(id);
}