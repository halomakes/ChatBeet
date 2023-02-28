using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SauceNET;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class SauceCommandModule : ApplicationCommandModule
{
    private readonly SauceNETClient _sauceClient;

    public SauceCommandModule(SauceNETClient sauceClient)
    {
        _sauceClient = sauceClient;
    }

    private async Task FindSauce(BaseContext ctx, string? imageUrl)
    {
        var results = await _sauceClient.GetSauceAsync(imageUrl);
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
            var response = await ctx.GetOriginalResponseAsync();
            await response.ModifyEmbedSuppressionAsync(true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Sorry, couldn't find anything for {imageUrl}, ya perv."));
    }

    [SlashCommand("sauce", "Get sauce for an image based on its url")]
    public Task DemandSauce(InteractionContext ctx, [Option("url", "URL of the image")] string imageUrl) => FindSauce(ctx, imageUrl);

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Find Sauce")]
    public async Task DemandSauce(ContextMenuContext ctx)
    {
        var embed = ctx.TargetMessage.Embeds.FirstOrDefault(e => e.Type == "image");
        if (embed is not null)
        {
            await FindSauce(ctx, embed.Image?.Url?.ToString() ?? embed.Url?.ToString());
            return;
        }

        var attachment = ctx.TargetMessage.Attachments.FirstOrDefault(a => a.MediaType.StartsWith("image", StringComparison.InvariantCultureIgnoreCase));
        if (attachment is not null)
        {
            await FindSauce(ctx, attachment.Url);
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Didn't see any image embeds on that message."));
    }
}
