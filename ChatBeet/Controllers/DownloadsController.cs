using System.Threading.Tasks;
using ChatBeet.Models;
using GravyBot;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class DownloadsController : ControllerBase
{
    private readonly MessageQueueService _queueService;
    private readonly IMediator _mediator;

    public DownloadsController(MessageQueueService queueService, IMediator mediator)
    {
        this._queueService = queueService;
        _mediator = mediator;
    }

    [HttpPost, Route("Complete")]
    public async Task<IActionResult> CompleteDownload([FromForm] DownloadCompleteMessage message)
    {
        _queueService.Push(message);
        await _mediator.Publish(message);
        return Ok();
    }
}