using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;
public class WhipCommandModule : ApplicationCommandModule
{
    private static readonly TimeSpan ComparisonWindow = TimeSpan.FromMinutes(10);
    private static Dictionary<ulong, DiscordMessage> LastPosts = new();
    private readonly DiscordClient _client;
    private const char prefix = '8';
    private const char suffix = 'D';
    private const char body = '=';
    private const int headerLines = 3;
    private static readonly int maxLength = 16;
    private static readonly Random rng = new();
    private static readonly int godChance = 10_000_000;
    private static readonly int godLength = 32;

    public WhipCommandModule(DiscordClient client)
    {
        _client = client;
    }

    [SlashCommand("epeen", "Compare lengths to determine who wins an argument.")]
    public async Task WhipOut(InteractionContext ctx)
    {
        if (IsPostValid(ctx.Channel.Id))
        {
            if (HasUserAlready(ctx.Channel.Id, ctx.User))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("You must wait for the current comparison in this channel to end before rerolling."));
                return;
            }
            await UpdateComparison(ctx);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AsEphemeral()
                .WithContent("Post has been updated."));
        }
        else
        {
            await StartComparison(ctx);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .AsEphemeral()
            .WithContent("Comparison started. 🍆"));
        }
    }

    private async Task UpdateComparison(InteractionContext ctx)
    {
        var post = LastPosts[ctx.Channel.Id];
        var lines = post.Content.Split(Environment.NewLine, StringSplitOptions.None);
        var header = lines.Take(headerLines);
        var comparisons = lines.Skip(headerLines).ToList();
        comparisons.Add(GetRow(ctx.User));

        var newContent = string.Join(Environment.NewLine, header.Concat(comparisons.OrderByDescending(c => c.IndexOf(suffix))));

        LastPosts[ctx.Channel.Id] = await post.ModifyAsync(newContent);
    }

    private async Task StartComparison(InteractionContext ctx)
    {
        LastPosts[ctx.Channel.Id] = await _client.SendMessageAsync(ctx.Channel, @$"A size comparison has been started! Use {Formatter.InlineCode("/epeen")} to show them what you got.
This comparison will expire {Formatter.Timestamp(DateTime.Now + ComparisonWindow)}.

{GetRow(ctx.User)}");
    }

    private bool IsPostValid(ulong channelId) => LastPosts.TryGetValue(channelId, out var post) && (DateTime.Now - post.CreationTimestamp) <= ComparisonWindow;

    private bool HasUserAlready(ulong channelId, DiscordUser user) => LastPosts[channelId].Content.Split(Environment.NewLine).Any(l => l.EndsWith($" {Formatter.Mention(user)}"));

    private string GetRow(DiscordUser user) => $"{prefix}{GetBar(GetLength(), body)}{suffix} {Formatter.Mention(user)}";

    private int GetLength()
    {
        var isGodLength = rng.Next(0, godChance) == 0;
        return isGodLength ? godLength : rng.NormalNext(1, maxLength);
    }

    private static string GetBar(int length, char @char) => new(Enumerable.Repeat(@char, length).ToArray());
}
