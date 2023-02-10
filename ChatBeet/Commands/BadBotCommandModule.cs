using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BadBotCommandModule : ApplicationCommandModule
{
    private static DateTime? _lastReactionTime = null;
    private static readonly TimeSpan debounce = TimeSpan.FromSeconds(20);

    [SlashCommand("bad-bot", "Hurt ChatBeet's feelings")]
    public async Task BeHurt(InteractionContext ctx)
    {
        if (!_lastReactionTime.HasValue || (DateTime.Now - _lastReactionTime.Value) > debounce)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(Formatter.Italic("sad bot noises"))
                );
        }
        _lastReactionTime = DateTime.Now;
    }

    [SlashCommand("shit-bot", "Hurt ChatBeet's feelings")]
    public async Task BeVeryHurt(InteractionContext ctx)
    {
        if (!_lastReactionTime.HasValue || (DateTime.Now - _lastReactionTime.Value) > debounce)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(Formatter.Italic("very sad bot noises"))
                );
        }
        _lastReactionTime = DateTime.Now;
    }
}
