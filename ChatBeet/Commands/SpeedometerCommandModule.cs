using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;

namespace ChatBeet.Commands;
public class SpeedometerCommandModule : ApplicationCommandModule
{
    private static readonly TimeSpan Period = TimeSpan.FromMinutes(1);

    [SlashCommand("speed", "Get the message rate in the current channel")]
    public async Task GetMessageRate(InteractionContext ctx)
    {
        var cutoff = DateTime.Now - Period;
        var messages = await ctx.Channel.GetMessagesAsync(limit: 200);
        var rate = messages.Count(m => m.CreationTimestamp >= cutoff);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"There {(rate == 1 ? "has" : "have")} been {rate} {(rate == 1 ? "message" : "messages")} in {Formatter.Mention(ctx.Channel)} in the last {Period.Humanize()}"));
    }
}
