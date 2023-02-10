using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public partial class MessageTransformCommandModule : ApplicationCommandModule
{
    private readonly NegativeResponseService _negativeResponseService;
    private readonly DiscordClient _client;

    public MessageTransformCommandModule(NegativeResponseService negativeResponseService, DiscordClient client)
    {
        _negativeResponseService = negativeResponseService;
        _client = client;
    }

    [GeneratedRegex(@"([\x00-\x7F])")]
    private static partial Regex SpacingRegex();

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Kern")]
    public async Task Kern(ContextMenuContext ctx)
    {
        if (ctx.TargetMessage.Author.Equals(_client.CurrentUser))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(_negativeResponseService.GetResponseString()));
            return;
        }

        var message = ctx.TargetMessage.Content;
        var kerned = SpacingRegex().Replace(message, " $1").Replace("   ", "  ").Trim().ToUpperInvariant();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{Formatter.Mention(ctx.TargetMessage.Author)}: {kerned}"));
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Mock")]
    public async Task Mock(ContextMenuContext ctx)
    {
        if (ctx.TargetMessage.Author.Equals(_client.CurrentUser))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(_negativeResponseService.GetResponseString()));
            return;
        }

        var message = ctx.TargetMessage.Content;
        var mocked = string.Concat(RandomizeCase(message));

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{Formatter.Mention(ctx.TargetMessage.Author)}: {mocked}"));
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