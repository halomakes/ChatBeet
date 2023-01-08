using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public partial class MessageTransformCommandProcessor : ApplicationCommandModule
{
    private readonly NegativeResponseService negativeResponseService;
    private readonly DiscordClient client;

    public MessageTransformCommandProcessor(NegativeResponseService negativeResponseService, DiscordClient client)
    {
        this.negativeResponseService = negativeResponseService;
        this.client = client;
    }

    [GeneratedRegex(@"([\x00-\x7F])")]
    private static partial Regex SpacingRegex();

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Kern")]
    public async Task Kern(ContextMenuContext ctx)
    {
        if (ctx.TargetMessage.Author.Equals(client.CurrentUser))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(negativeResponseService.GetResponseString()));
            return;
        }

        var message = ctx.TargetMessage.Content;
        var kerned = SpacingRegex().Replace(message, " $1").Replace("   ", "  ").Trim().ToUpperInvariant();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(kerned));
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Mock")]
    public async Task Mock(ContextMenuContext ctx)
    {
        if (ctx.TargetMessage.Author.Equals(client.CurrentUser))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(negativeResponseService.GetResponseString()));
            return;
        }

        var message = ctx.TargetMessage.Content;
        var mocked = string.Concat(RandomizeCase(message));

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(mocked));
        await ctx.TargetMessage.CreateReactionAsync(DiscordEmoji.FromUnicode(@"🤣"));

        static IEnumerable<char> RandomizeCase(string s)
        {
            var rng = new Random();
            var switchProbability = .8; // 80% chance to change case each character
            var isUppercase = rng.Next(0, 2) > 0; // 50% probability for first character
            foreach (var @char in s)
            {
                if (rng.NextDouble() < switchProbability)
                    isUppercase = !isUppercase;

                yield return isUppercase ? char.ToUpper(@char) : char.ToLower(@char);
            }
        }
    }
}