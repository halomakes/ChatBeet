using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class TenorCommandProcessor : CommandProcessor
    {
        private readonly TenorGifService gifService;

        public TenorCommandProcessor(TenorGifService gifService)
        {
            this.gifService = gifService;
        }

        [Command("gif {query}", Description = "Search for a GIF on Tenor")]
        public async Task<IClientMessage> SearchGifs([Required] string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var url = await gifService.GetGifAsync(query);

                if (!string.IsNullOrEmpty(url))
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{url} - {IrcValues.AQUA}Via Tenor");
                else
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, couldn't find that anything for {IrcValues.ITALIC}{query.Trim()}{IrcValues.RESET}.");
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Please provide a search term.");
            }
        }
    }
}
