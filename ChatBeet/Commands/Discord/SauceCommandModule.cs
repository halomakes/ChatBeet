using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SauceNET;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class SauceCommandModule : ApplicationCommandModule
{
    private readonly SauceNETClient sauceClient;

    public SauceCommandModule(SauceNETClient sauceClient)
    {
        this.sauceClient = sauceClient;
    }

    private async Task FindSauce(BaseContext ctx, string imageUrl)
    {
        var results = await sauceClient.GetSauceAsync(imageUrl);
        var bestMatches = results?.Results?.OrderByDescending(r => double.TryParse(r.Similarity, out var p) ? p : 0).Take(3).ToList();
        if (bestMatches?.Any() ?? false)
        {
            var content = bestMatches.Select(m =>
            {
                var percentage = double.Parse(m.Similarity);
                var percentageDesc = Formatter.Bold($"{percentage:F}%");
                return $"{percentageDesc} match on {Formatter.Bold(m.DatabaseName)}: {m.SourceURL}";
            });

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(string.Join(Environment.NewLine, content)));
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find anything for {imageUrl}, ya perv."));
    }

    [SlashCommand("sauce", "Get sauce for an image based on its url.")]
    public Task DemandSauce(InteractionContext ctx, [Option("url", "URL of the image")] string imageUrl) => FindSauce(ctx, imageUrl);

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Find Sauce")]
    public async Task DemandSauce(ContextMenuContext ctx)
    {
        var embed = ctx.TargetMessage.Embeds.FirstOrDefault(e => e.Type == "image");
        if (embed is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Didn't see any image embeds on that message."));
            return;
        }
        await FindSauce(ctx, embed.Image?.Url?.ToString() ?? embed.Url?.ToString());
    }
}
