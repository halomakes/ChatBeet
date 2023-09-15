using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using Dice;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SixLabors.ImageSharp;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class DiceCommandModule : ApplicationCommandModule
{
    private readonly GraphicsService _graphics;
    private readonly IUsersRepository _usersRepository;
    private readonly UserPreferencesService _preferencesService;
    
    private const int MaxRollsInImage = 8;

    public DiceCommandModule(GraphicsService graphics, UserPreferencesService preferencesService, IUsersRepository usersRepository)
    {
        _graphics = graphics;
        _preferencesService = preferencesService;
        _usersRepository = usersRepository;
    }

    [SlashCommand("roll", "Roll some dice")]
    public async Task RollDice(InteractionContext ctx, [Option("dice", "Information about dice to roll (e.g. 2d20)")] string dice)
    {
        try
        {
            var rollResult = Roller.Roll(dice);
            if (rollResult.NumRolls > MaxRollsInImage)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .WithContent(rollResult.ToString()));
                return;
            }

            var user = await _usersRepository.GetUserAsync(ctx.User);
            var userColor = await _preferencesService.Get(user.Id, UserPreference.GearColor);
            Color? parsedColor = Color.TryParse(userColor, out var successfulParse) ? successfulParse : null;
            await using var graphic = await _graphics.BuildDiceRollImageAsync(rollResult, parsedColor);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(rollResult.ToString())
                    .AddFile("roll-result.webp", graphic));

        }
        catch (DiceException)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent($"Unable to parse roll input \"{dice}\" - See https://skizzerz.net/DiceRoller/Dice_Reference"));
        }
    }
}