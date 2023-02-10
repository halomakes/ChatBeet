using ChatBeet.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using ChatBeet.Services;

namespace ChatBeet.Pages;

public class SpeedometerModel : PageModel
{
    private readonly ChatBeetConfiguration _config;

    public int Rate { get; set; }
    public string ChannelName { get; set; }

    public SpeedometerModel(IOptions<ChatBeetConfiguration> options)
    {
        _config = options.Value;
    }

    public void OnGet([FromQuery] string channel)
    {
        Rate = SpeedometerService.GetRecentMessageCount(default, TimeSpan.FromMinutes(1));
    }
}