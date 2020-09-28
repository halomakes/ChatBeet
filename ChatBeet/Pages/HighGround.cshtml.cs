using ChatBeet.Models;
using ChatBeet.Rules;
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
            if (HighGroundRule.HighestNicks.AsEnumerable().Any())
            {
                var top = HighGroundRule.HighestNicks.AsEnumerable().PickRandom();
                Nick = top.Value;
                Channel = top.Key;
            }
        }

        public IActionResult OnPost()
        {
            if (User?.Identity?.IsAuthenticated ?? false && !string.IsNullOrEmpty(Channel) && Channel.StartsWith("#"))
            {
                queueService.Push(new HighGroundClaim { Nick = User.GetNick(), Channel = Channel });
            }
            return RedirectToPage("/HighGround");
        }
    }
}
