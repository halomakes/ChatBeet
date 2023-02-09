using ChatBeet.Attributes;
using ChatBeet.Converters;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using SauceNET;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc;

public class SauceCommandProcessor : CommandProcessor
{
    private readonly SauceNETClient sauceClient;

    public SauceCommandProcessor(SauceNETClient sauceClient)
    {
        this.sauceClient = sauceClient;
    }

    [Command("sauce {imageUrl}", Description = "Get sauce for an image based on its url.")]
    public async Task<IClientMessage> DemandSauce([Required, Uri, TypeConverter(typeof(UrlTypeConverter))] Uri imageUrl) // lol
    {
        var results = await sauceClient.GetSauceAsync(imageUrl.AbsolutePath);
        var bestMatch = results?.Results?.OrderByDescending(r => double.TryParse(r.Similarity, out var p) ? p : 0).FirstOrDefault();
        if (bestMatch != default)
        {
            var percentage = double.Parse(bestMatch.Similarity);
            var percentageDesc = $"{IrcValues.BOLD}{percentage:F}%".Colorize(Convert.ToInt32(percentage));

            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{percentageDesc} match on {IrcValues.BOLD}{bestMatch.DatabaseName}{IrcValues.RESET}: {bestMatch.SourceURL}");
        }
        return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything for {imageUrl}, ya perv.");
    }
}