using ChatBeet.Commands.Discord.Autocomplete;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GravyBot;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class MemoryCellCommandModule : ApplicationCommandModule
{
    private readonly MemoryCellContext dbContext;
    private readonly MessageQueueService queue;

    public MemoryCellCommandModule(MemoryCellContext dbContext, MessageQueueService queue)
    {
        this.dbContext = dbContext;
        this.queue = queue;
    }

    [SlashCommand("who-def", "Check who set a peasant definition")]
    public async Task GetAuthor(InteractionContext ctx, [Option("key", "Key of the entry to look up"), Autocomplete(typeof(MemoryCellAutocompleteProvider))] string key)
    {
        var cell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());

        if (cell is not null)
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{Formatter.Bold(cell.Key)} was set by {Formatter.Bold(cell.Author)}.")
        );
        else
            await NotFound(ctx, key);
    }

    [SlashCommand("recall", "Get the value of a peasant definition")]
    public async Task GetCell(InteractionContext ctx, [Option("key", "Key of the entry to look up"), Autocomplete(typeof(MemoryCellAutocompleteProvider))] string key)
    {
        var cell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());

        if (cell is not null)
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{Formatter.Bold(cell.Key)}: {cell.Value}"));
        else
            await NotFound(ctx, key);
    }

    [SlashCommand("remember", "Create or replace a peasant definition")]
    public async Task SetCell(InteractionContext ctx, [Option("key", "Key of the entry to set")] string key, [Option("value", "Value to store")] string value)
    {
        var existingCell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());
        if (existingCell != null)
        {
            dbContext.MemoryCells.Remove(existingCell);
            await dbContext.SaveChangesAsync();
        }

        dbContext.MemoryCells.Add(new MemoryCell
        {
            Author = ctx.User.Username,
            Key = key,
            Value = value
        });
        await dbContext.SaveChangesAsync();

        if (existingCell is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent("Got it! 👍"));
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(@$"Got it! 👍
Previous value was {Formatter.Bold(existingCell.Value)}, set by {existingCell.Author}."));
        }

        if (ctx.Channel.IsPrivate)
        {
            queue.Push(new DefinitionChange
            {
                Key = key,
                NewNick = ctx.User.Username,
                NewValue = value,
                OldNick = existingCell?.Author,
                OldValue = existingCell?.Value
            });

        }
    }

    [SlashCommand("append", "Add something on to an existing definition")]
    public async Task AppendCell(InteractionContext ctx, [Option("key", "Key of the entry to set")] string key, [Option("value", "Value to append"), Autocomplete(typeof(MemoryCellAutocompleteProvider))] string value)
    {
        var cell = await dbContext.MemoryCells.FirstOrDefaultAsync(c => c.Key.ToLower() == key.ToLower());
        if (cell is not null)
            await SetCell(ctx, key, $"{cell.Value} | {value.Trim()}");
        else
            await NotFound(ctx, key);
    }

    private Task NotFound(InteractionContext ctx, string key) =>
        ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"I don't have anything for {Formatter.Bold(key)}.")
    );
}
