using ChatBeet.Models;
using GravyBot;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class DownloadsController : ControllerBase
    {
        private readonly MessageQueueService queueService;

        public DownloadsController(MessageQueueService queueService)
        {
            this.queueService = queueService;
        }

        [HttpPost, Route("Complete")]
        public IActionResult CompleteDownload([FromForm] DownloadCompleteMessage message)
        {
            queueService.Push(message);
            return Ok();
        }
    }
}
