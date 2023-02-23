using System.Threading.Tasks;
using ChatBeet.Notifications;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BonkCommandModule : ApplicationCommandModule
{
    private readonly DiscordClient _client;
    private readonly MemeService _memes;
    private readonly IMediator _mediator;

    public BonkCommandModule(DiscordClient client, MemeService memes, IMediator mediator)
    {
        _client = client;
        _memes = memes;
        _mediator = mediator;
    }

    [SlashCommand("bonk", "Call someone out for being horni")]
    public async Task Bonk(InteractionContext ctx, [Option("user", "Person to compliment")] DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"🚨🚨 {Formatter.Mention(user)} has been reported as being horny 🚨🚨  {Formatter.Mention(_client.CurrentUser)} is now contacting the {Formatter.Bold("FBI")}, {Formatter.Bold("NSA")}, {Formatter.Bold("CIA")}, {Formatter.Bold("Navy SEALs")}, {Formatter.Bold("Secret Service")}, and {Formatter.Bold("ur mom")}. ")
        );
        await BonkReactAsync(user, ctx.Channel);
        await EmbedImageAsync(await ctx.GetOriginalResponseAsync());
        await _mediator.Publish(new BonkNotification(ctx.Guild.Id, ctx.User, user));
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "Bonk")]
    public async Task Bonk(ContextMenuContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"🚨🚨 {Formatter.Mention(ctx.TargetUser)} has been reported as being horny 🚨🚨  {Formatter.Mention(_client.CurrentUser)} is now contacting the {Formatter.Bold("FBI")}, {Formatter.Bold("NSA")}, {Formatter.Bold("CIA")}, {Formatter.Bold("Navy SEALs")}, {Formatter.Bold("Secret Service")}, and {Formatter.Bold("ur mom")}. ")
        );
        await BonkReactAsync(ctx.TargetUser, ctx.Channel);
        await EmbedImageAsync(await ctx.GetOriginalResponseAsync());
        await _mediator.Publish(new BonkNotification(ctx.Guild.Id, ctx.User, ctx.TargetUser));
    }

    private async Task EmbedImageAsync(DiscordMessage message)
    {
        var image = await _memes.GetRandomImageAsync("bonk");
        await message.ModifyAsync(new DiscordMessageBuilder()
            .WithContent(message.Content)
            .AddEmbed(new DiscordEmbedBuilder()
                .WithImageUrl(image)));
    }

    private async Task BonkReactAsync(DiscordUser user, DiscordChannel channel)
    {
        var messages = await channel.GetMessagesAsync();
        var message = messages
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefault(m => m.Author == user);
        if (message is not null && DiscordEmoji.TryFromName(_client, ":bonk:", out var emoji))
            await message.CreateReactionAsync(emoji);
    }
}