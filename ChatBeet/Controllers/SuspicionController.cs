using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
[ResponseCache(Duration = 300)]
public class SuspicionController : Controller
{
    private readonly SuspicionService _suspicionService;

    public SuspicionController(SuspicionService suspicionService)
    {
        _suspicionService = suspicionService;
    }

    /// <summary>
    /// Get current suspicion levels
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SuspicionRank>>> GetSuspicionLevels() => Ok(await _suspicionService.GetSuspicionLevels());
}