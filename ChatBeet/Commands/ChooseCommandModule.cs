using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

public class ChooseCommandModule : ApplicationCommandModule
{
    public const string Id = "choose";

    [SlashCommand("choose", "Choose randomly from a set of options")]
    public async Task PromptChoice(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
            .WithTitle("Choose Option")
            .WithContent($"Enter the options for {ctx.Client.CurrentUser.Mention} to choose between, one per line.")
            .WithCustomId(Id)
            .AddComponents(
                new TextInputComponent("Options", "options", style: TextInputStyle.Paragraph, placeholder: @"Option A
Option B
Option C",required: true)));
    }
}