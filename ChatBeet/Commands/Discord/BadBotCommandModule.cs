using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BadBotCommandModule : ApplicationCommandModule
{
    private static DateTime? lastReactionTime = null;
    private static readonly TimeSpan debounce = TimeSpan.FromSeconds(20);

    [SlashCommand("bad-bot", "Hurt ChatBeet's feelings.")]
    public async Task BeHurt(InteractionContext ctx)
    {
        if (!lastReactionTime.HasValue || (DateTime.Now - lastReactionTime.Value) > debounce)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(Formatter.Italic("sad bot noises"))
                );
        }
        lastReactionTime = DateTime.Now;
    }

    [SlashCommand("shit-bot", "Hurt ChatBeet's feelings.")]
    public async Task BeVeryHurt(InteractionContext ctx)
    {
        if (!lastReactionTime.HasValue || (DateTime.Now - lastReactionTime.Value) > debounce)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(Formatter.Italic("very sad bot noises"))
                );
        }
        lastReactionTime = DateTime.Now;
    }
}
