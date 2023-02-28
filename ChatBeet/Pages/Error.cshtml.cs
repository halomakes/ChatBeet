using ChatBeet.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Threading.Tasks;
using ChatBeet.Notifications;
using MediatR;

namespace ChatBeet.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    private readonly IMediator _service;
    private readonly DiscordLogService _discord;

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public ErrorModel(IMediator service, DiscordLogService discord)
    {
        _service = service;
        _discord = discord;
    }

    public async Task OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error != default)
        {
            await _service.Publish(new WebExceptionNotification(exceptionHandlerPathFeature.Error));
            await _discord.LogError("WebUI Error", exceptionHandlerPathFeature.Error);
        }
    }
}