using ChatBeet.Irc;
using DtellaRules.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBeet.Server.Controllers
{
    [Route("api/[controller]")]
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
