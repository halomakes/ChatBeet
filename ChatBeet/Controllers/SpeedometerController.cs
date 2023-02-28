using Microsoft.AspNetCore.Mvc;
using ChatBeet.Services;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpeedometerController : ControllerBase
{
    /// <summary>
    /// Get the current message rate in a channel
    /// </summary>
    /// <param name="channelId">ID of channel to check</param>
    /// <param name="timeSpan">Span of time to check over (default 1 minute)</param>
    /// <returns>Number of messages received in specified period of time</returns>
    [HttpGet("{channelId}")]
    public ActionResult<int> GetChannelMessageRate([FromRoute] ulong channelId, [FromQuery] TimeSpan? timeSpan)
    {
        var period = timeSpan ?? TimeSpan.FromMinutes(1);
        return Ok(SpeedometerService.GetRecentMessageCount(channelId, period));
    }
}