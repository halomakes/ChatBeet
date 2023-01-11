using ChatBeet.Services;
using GravyBot;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChatBeet.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }
        private readonly MessageQueueService service;
        private readonly DiscordLogService _discord;

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger, MessageQueueService service, DiscordLogService discord)
        {
            _logger = logger;
            this.service = service;
            _discord = discord;
        }

        public async Task OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error != default)
            {
                service.Push(exceptionHandlerPathFeature.Error);
                await _discord.LogError("WebUI Error", exceptionHandlerPathFeature.Error);
            }
        }
    }
}
