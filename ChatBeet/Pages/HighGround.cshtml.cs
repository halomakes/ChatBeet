using ChatBeet.Commands;
using ChatBeet.Utilities;
using GravyBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ChatBeet.Pages
{
    public class HighGroundModel : PageModel
    {
        private readonly MessageQueueService queueService;

        public string Nick { get; set; }

        [BindProperty]
        public string Channel { get; set; }

        public HighGroundModel(MessageQueueService queueService)
        {
            this.queueService = queueService;
        }

        public void OnGet()
        {
            if (HighGroundCommandProcessor.HighestNicks.AsEnumerable().Any())
            {
                var top = HighGroundCommandProcessor.HighestNicks.AsEnumerable().PickRandom();
                Nick = top.Value;
                Channel = top.Key;
            }
        }
    }
}
