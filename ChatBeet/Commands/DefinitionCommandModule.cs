using System.Threading.Tasks;
using ChatBeet.Commands.Autocomplete;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Notifications;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class DefinitionCommandModule : ApplicationCommandModule
{
    private readonly IDefinitionsRepository _dbContext;
    private readonly IUsersRepository _users;
    private readonly IMediator _queue;

    public DefinitionCommandModule(IDefinitionsRepository dbContext, IMediator queue, IUsersRepository users)
    {
        _dbContext = dbContext;
        _queue = queue;
        _users = users;
    }

    [SlashCommand("who-def", "Check who set a peasant definition")]
    public async Task GetAuthor(InteractionContext ctx, [Option("key", "Key of the entry to look up"), Autocomplete(typeof(MemoryCellAutocompleteProvider))] string key)
    {
        var cell = await _dbContext.Definitions
            .Include(d => d.Author)
            .FirstOrDefaultAsync(c => c.GuildId == ctx.Guild.Id && c.Key.ToLower() == key.ToLower());

        if (cell is not null)
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Bold(cell.Key)} was set by {Formatter.Bold(cell.Author?.Mention())}.")
            );
        else
            await NotFound(ctx, key);
    }

    [SlashCommand("recall", "Get the value of a peasant definition")]
    public async Task GetCell(InteractionContext ctx, [Option("key", "Key of the entry to look up"), Autocomplete(typeof(MemoryCellAutocompleteProvider))] string key)
    {
        var cell = await _dbContext.Definitions.FirstOrDefaultAsync(c => c.GuildId == ctx.Guild.Id && c.Key.ToLower() == key.ToLower());

        if (cell is not null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Bold(cell.Key)}: {cell.Value}"));
            await _queue.Publish(new BonkableMessageNotification(await ctx.GetOriginalResponseAsync()));
        }
        else
            await NotFound(ctx, key);
    }

    [SlashCommand("remember", "Create or replace a peasant definition")]
    public async Task SetCell(InteractionContext ctx, [Option("key", "Key of the entry to set")] string key, [Option("value", "Value to store")] string value)
    {
        var existingCell = await _dbContext.Definitions
            .Include(d => d.Author)
            .FirstOrDefaultAsync(c => c.GuildId == ctx.Guild.Id && c.Key.ToLower() == key.ToLower());
        if (existingCell != null)
        {
            _dbContext.Definitions.Remove(existingCell);
            await _dbContext.SaveChangesAsync();
        }

        var user = await _users.GetUserAsync(ctx.User);
        _dbContext.Definitions.Add(new Definition
        {
            CreatedBy = user.Id,
            Key = key,
            Value = value,
            GuildId = ctx.Guild.Id,
            CreatedAt = existingCell?.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        if (existingCell is null)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent("Got it! 👍"));
        }
        else
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(@$"Got it! 👍
Previous value was {Formatter.Bold(existingCell.Value)}, set by {existingCell.Author?.Mention()}."));
            await _queue.Publish(new BonkableMessageNotification(await ctx.GetOriginalResponseAsync()));
        }

        if (ctx.Channel.IsPrivate)
        {
            await _queue.Publish(new DefinitionChange
            {
                Key = key,
                NewUser = user,
                NewValue = value,
                OldUser = existingCell?.Author,
                OldValue = existingCell?.Value,
                GuildId = ctx.Guild.Id
            });
        }
    }

    [SlashCommand("append", "Add something on to an existing definition")]
    public async Task AppendCell(InteractionContext ctx, [Option("key", "Key of the entry to set")] string key, [Option("value", "Value to append"), Autocomplete(typeof(MemoryCellAutocompleteProvider))] string value)
    {
        var cell = await _dbContext.Definitions.FirstOrDefaultAsync(c => c.GuildId == ctx.Guild.Id && c.Key.ToLower() == key.ToLower());
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