using System.Threading.Tasks;
using ChatBeet.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class DownloadsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DownloadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost, Route("Complete")]
    public async Task<IActionResult> CompleteDownload([FromForm] DownloadCompleteMessage message)
    {
        await _mediator.Publish(message);
        return Ok();
    }
}