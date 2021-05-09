using GravyBot;
using GravyBot.Commands;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ChatBeet.Pages.Commands
{
    public class IndexModel : PageModel
    {
        public readonly ICommandOrchestratorBuilder Builder;
        public string CommandPrefix;

        public IndexModel(ICommandOrchestratorBuilder builder, IOptions<IrcBotConfiguration> options)
        {
            this.Builder = builder;
            CommandPrefix = options.Value.CommandPrefix;
        }

        public void OnGet()
        {
        }
    }
}
