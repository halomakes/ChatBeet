using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpeedometerController : ControllerBase
{
    private readonly SpeedometerService service;

    public SpeedometerController(SpeedometerService service)
    {
        this.service = service;
    }

    /// <summary>
    /// Get the current message rate in a channel
    /// </summary>
    /// <param name="channelName">Name of channel to check</param>
    /// <param name="timeSpan">Span of time to check over (default 1 minute)</param>
    /// <returns>Number of messages recieved in specified period of time</returns>
    [HttpGet("{channelName}")]
    public ActionResult<int> GetChannelMessageRate([FromRoute] string channelName, [FromQuery] TimeSpan? timeSpan)
    {
        var period = timeSpan ?? TimeSpan.FromMinutes(1);
        if (!channelName.StartsWith('#'))
            channelName = string.Concat('#', channelName);
        return Ok(service.GetRecentMessageCount(channelName, period));
    }
}