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

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Transform")]
    public async Task TransformMessage(ContextMenuContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
            .WithTitle("Verify Your Account")
            .WithCustomId($"message-transform-{ctx.TargetMessage.Id}")
            .AddComponents(new DiscordSelectComponent("replaceType", "Basic", options: new[]
            {
                new DiscordSelectComponentOption("Basic", "basic", "Use basic text replacement", true),
                new DiscordSelectComponentOption("Regular Expressions", "regex", "Use regular expressions in replacement")
            }))
            .AddComponents(new TextInputComponent("Text to Replace", "replacePattern", required: false))
            .AddComponents(new TextInputComponent("Replacement Text", "replaceValue", placeholder: "poop", value: "poop", required: false))
            .AddComponents(new DiscordSelectComponent("scramble", "Do Not Scramble", options: new[]
            {
                new DiscordSelectComponentOption("Do Not Scramble", "false", "Leave the order of words alone", true),
                new DiscordSelectComponentOption("Scramble Words", "true", "around the Mix order words of")
            }))
            .AddComponents(new DiscordSelectComponent("mock", "Do Not Mock", options: new[]
            {
                new DiscordSelectComponentOption("Do Not Mock", "false", "Leave the text case alone", true),
                new DiscordSelectComponentOption("Mock", "true", "rAndOmiZE tHe cAsE Of tHE mESsaGe tExT")
            }))
            .AddComponents(new DiscordSelectComponent("kern", "Do Not Kern", options: new[]
            {
                new DiscordSelectComponentOption("Do Not Kern", "false", "Leave text spacing alone", true),
                new DiscordSelectComponentOption("Kern Text", "true", "S p a c e  l e t t e r s  a p a r t")
            }))
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