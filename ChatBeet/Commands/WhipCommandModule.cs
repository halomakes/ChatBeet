using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

public class WhipCommandModule : ApplicationCommandModule
{
    private static readonly TimeSpan ComparisonWindow = TimeSpan.FromMinutes(10);
    private static readonly Dictionary<ulong, DiscordMessage> LastPosts = new();
    private readonly DiscordClient _client;
    private const char Prefix = '8';
    private const char Suffix = 'D';
    private const char Body = '=';
    private const int HeaderLines = 3;
    private static readonly int maxLength = 16;
    private static readonly Random rng = new();
    private static readonly int godChance = 10_000_000;
    private static readonly int godLength = 32;
    public const string Emoji = "🍆";

    public WhipCommandModule(DiscordClient client)
    {
        _client = client;
    }

    public static bool CanUpdate(DiscordMessage message, DiscordUser user) => LastPosts.Any(p => p.Value.Id == message.Id && IsPostInWindow(p.Value) && !HasUserAlready(message.Channel.Id, user));

    [SlashCommand("epeen", "Compare lengths to determine who wins an argument")]
    public async Task WhipOut(InteractionContext ctx)
    {
        if (IsPostValid(ctx.Channel.Id))
        {
            if (HasUserAlready(ctx.Channel.Id, ctx.User))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("You must wait for the current comparison in this channel to end before re-rolling."));
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
                .WithContent($"Comparison started. {Emoji}"));
        }
    }

    private static Task UpdateComparison(BaseContext ctx) => UpdateComparison(ctx.Channel.Id, ctx.User);

    public static async Task UpdateComparison(ulong channelId, DiscordUser user)
    {
        var post = LastPosts[channelId];
        var lines = post.Content.Split(Environment.NewLine, StringSplitOptions.None);
        var header = lines.Take(HeaderLines);
        var comparisons = lines.Skip(HeaderLines).ToList();
        comparisons.Add(GetRow(user));

        var newContent = string.Join(Environment.NewLine, header.Concat(comparisons.OrderByDescending(c => c.IndexOf(Suffix))));

        LastPosts[channelId] = await post.ModifyAsync(newContent);
    }

    private async Task StartComparison(BaseContext ctx)
    {
        LastPosts[ctx.Channel.Id] = await _client.SendMessageAsync(ctx.Channel, @$"A size comparison has been started! Use {Formatter.InlineCode("/epeen")} or {Emoji} react to show them what you got.
This comparison expires {Formatter.Timestamp(DateTime.Now + ComparisonWindow)}.

{GetRow(ctx.User)}");
        await LastPosts[ctx.Channel.Id].CreateReactionAsync(DiscordEmoji.FromUnicode(Emoji));
    }

    private static bool IsPostValid(ulong channelId) => LastPosts.TryGetValue(channelId, out var post) && IsPostInWindow(post);

    private static bool IsPostInWindow(DiscordMessage post) => (DateTime.Now - post.CreationTimestamp) <= ComparisonWindow;

    private static bool HasUserAlready(ulong channelId, DiscordUser user) => LastPosts[channelId].Content.Split(Environment.NewLine).Any(l => l.EndsWith($" {Formatter.Mention(user)}"));

    private static string GetRow(DiscordUser user) => $"{Prefix}{GetBar(GetLength(), Body)}{Suffix} {Formatter.Mention(user)}";

    private static int GetLength()
    {
        var isGodLength = rng.Next(0, godChance) == 0;
        return isGodLength ? godLength : rng.NormalNext(1, maxLength);
    }

    private static string GetBar(int length, char @char) => new(Enumerable.Repeat(@char, length).ToArray());
}