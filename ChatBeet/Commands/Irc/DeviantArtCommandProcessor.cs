using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc;

public class DeviantArtCommandProcessor : CommandProcessor
{
    private readonly DeviantartService daService;

    public DeviantArtCommandProcessor(DeviantartService daService)
    {
        this.daService = daService;
    }

    [Command("da {query}", Description = "Get a random deviation from DeviantArt.")]
    [Command("deviantart {query}", Description = "Get a random deviation from DeviantArt.")]
    public async Task<IClientMessage> GetRandomDeviation([Required] string query)
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