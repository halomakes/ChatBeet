using ChatBeet.Configuration;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;

namespace ChatBeet.Pages
{
    public class SpeedometerModel : PageModel
    {
        private readonly SpeedometerService speedoService;
        private readonly ChatBeetConfiguration config;

        public int Rate { get; set; }
        public string ChannelName { get; set; }

        public SpeedometerModel(SpeedometerService speedoService, IOptions<ChatBeetConfiguration> options)
        {
            this.speedoService = speedoService;
            config = options.Value;
        }

        public void OnGet([FromQuery] string channel)
        {
            ChannelName = string.IsNullOrEmpty(channel) ? config.MainChannel : channel.Trim();
            Rate = speedoService.GetRecentMessageCount(ChannelName, TimeSpan.FromMinutes(1));
        }
    }
}
