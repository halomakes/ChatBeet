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
    public const string Prefix = "message-replace-";
    
    private readonly NegativeResponseService _negativeResponseService;
    private readonly DiscordClient _client;

    public MessageTransformCommandModule(NegativeResponseService negativeResponseService, DiscordClient client)
    {
        _negativeResponseService = negativeResponseService;
        _client = client;
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Replace Text")]
    public async Task PromptReplacement(ContextMenuContext ctx)
    {
        if (ctx.TargetMessage.Author.Equals(_client.CurrentUser))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(_negativeResponseService.GetResponseString()));
            return;
        }
        
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
            .WithTitle("Replace Text")
            .WithCustomId($"{Prefix}{ctx.TargetMessage.Id}")
            .AddComponents(new TextInputComponent("Text to Replace", "pattern", style: TextInputStyle.Short, required: false))
            .AddComponents(new TextInputComponent("Replacement Text", "value", style: TextInputStyle.Short, placeholder: "poop", value: "poop", required: false))
            .AddComponents(new TextInputComponent("Use Regular Expression", "regex", style: TextInputStyle.Short, value:"false", required: false))
            .AddComponents(new TextInputComponent("Ignore Case", "ignoreCase", style: TextInputStyle.Short, value:"false", required: false))
            .WithContent($@"Fill out the fields below to specify how to modify this message
{Formatter.BlockCode(ctx.TargetMessage.Content)}")
        );
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