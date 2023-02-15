using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Exceptions;
using ChatBeet.Notifications;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
[SlashCommandGroup("karma", "Commands related to the karma system")]
public class KarmaCommandModule : ApplicationCommandModule
{
    private readonly KarmaService _karma;
    private readonly IUsersRepository _usersRepository;
    private readonly IMediator _mediator;

    public KarmaCommandModule(KarmaService karma, IUsersRepository usersRepository, IMediator mediator)
    {
        _karma = karma;
        _usersRepository = usersRepository;
        _mediator = mediator;
    }

    [SlashCommand("check", "Check a karma level")]
    public async Task Check(InteractionContext ctx, [Option("key", "Key of the entry to look up")] string key)
    {
        key = key.Trim();
        var level = await _karma.GetLevel(ctx.Guild.Id, key);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{key.ToPossessive()} karma is {level}."));
    }

    [SlashCommand("increment", "Increase a karma level")]
    public async Task Increase(InteractionContext ctx, [Option("key", "Key of the entry to vote on")] string key)
    {
        var currentUser = await _usersRepository.GetUserAsync(ctx.User);
        key = key.Trim();
        try
        {
            await _karma.Increment(ctx.Guild.Id, key, currentUser);
            var level = await _karma.GetLevel(ctx.Guild.Id, key);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{key.ToPossessive()} karma is now {level}."));
            await _mediator.Publish(new KarmaChangeNotification(await ctx.GetOriginalResponseAsync(), await _karma.GetCanonicalKey(ctx.Guild.Id, key), level, level - 1));
        }
        catch (KarmaRateLimitException e)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"You can change {key.ToPossessive()} karma again {Formatter.Timestamp(e.Delay)}.")
                .AsEphemeral());
        }
    }

    [SlashCommand("decrement", "Decrease a karma level")]
    public async Task Decrease(InteractionContext ctx, [Option("key", "Key of the entry to vote on")] string key)
    {
        var currentUser = await _usersRepository.GetUserAsync(ctx.User);
        key = key.Trim();
        try
        {
            await _karma.Decrement(ctx.Guild.Id, key, currentUser);
            var level = await _karma.GetLevel(ctx.Guild.Id, key);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{key.ToPossessive()} karma is now {level}."));
            await _mediator.Publish(new KarmaChangeNotification(await ctx.GetOriginalResponseAsync(), await _karma.GetCanonicalKey(ctx.Guild.Id, key), level, level + 1));
        }
        catch (KarmaRateLimitException e)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"You can change {key.ToPossessive()} karma again {Formatter.Timestamp(e.Delay)}.")
                .AsEphemeral());
        }
    }
}