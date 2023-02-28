using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

public class PollCommandModule : ApplicationCommandModule
{
    public const string Id = "poll";

    [SlashCommand("poll", "Conduct a poll")]
    public async Task PromptPoll(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
            .WithTitle("Initiate Poll")
            .WithContent("Enter your poll description and options below.  Put an emoji and space before your option label to customize reactions.  One option per line.")
            .WithCustomId(Id)
            .AddComponents(new TextInputComponent("Poll Description", "description", style: TextInputStyle.Short, required: true))
            .AddComponents(new TextInputComponent("Options", "options", style: TextInputStyle.Paragraph, placeholder: @"1️⃣ miatas
2️⃣ more miatas
3️⃣ all the miatas", required: true)));
    }
}