using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class DeviantArtCommandProcessor : CommandProcessor
    {
        private readonly DeviantartService daService;

        public DeviantArtCommandProcessor(DeviantartService daService)
        {
            this.daService = daService;
        }

        [Command("da {query}")]
        [Command("deviantart {query}")]
        [ChannelPolicy("NoMain")]
        public async Task<IClientMessage> GetRandomDeviation(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var media = await daService.GetRecentImageAsync(query);

                if (media != null)
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{IrcValues.BOLD}{media.Title?.Text}{IrcValues.RESET} - {media.Id}");
                else
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything matching {query}.");
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Please provide a search term.");
            }
        }
    }
}
