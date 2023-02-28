using System.Threading.Tasks;
using ChatBeet.Exceptions;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class HighGroundCommandModule : ApplicationCommandModule
{
    private readonly MustafarService _mustafar;
    private readonly GraphicsService _graphics;
    
    public HighGroundCommandModule(GraphicsService graphics, MustafarService mustafar)
    {
        _graphics = graphics;
        _mustafar = mustafar;
    }

    [SlashCommand("jump", "Claim the high ground")]
    public async Task Claim(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        try
        {
            var change = await _mustafar.ClaimAsync(ctx.Guild.Id, ctx.User);
            if (change.Previous is null)
            {
                await using var graphic = await _graphics.BuildHighGroundImageAsync($"#{ctx.Channel.Name}", ctx.User.Username);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"{Formatter.Mention(ctx.User)} has the high ground.")
                    .AddFile("high-ground.webp", graphic));
            }
            else
            {
                await using var graphic = await _graphics.BuildHighGroundImageAsync(change.Previous.Discord!.Name!, ctx.User.Username);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"It's over, {change.Previous.Mention()}! {Formatter.Mention(ctx.User)} has the high ground!")
                    .AddFile("high-ground.webp", graphic));
            }
        }
        catch (WimpyLegsException e)
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Shouldn't have skipped leg day.  You will be ready to jump again {Formatter.Timestamp(e.NextJump)}."));
        }
        catch (StumbleException)
        {
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"{Formatter.Mention(ctx.User)} trips and falls off the high ground."));
        }
    }
}