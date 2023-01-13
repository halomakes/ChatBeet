using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Genbox.WolframAlpha;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class WolframCommandModule : ApplicationCommandModule
{
    private readonly WolframAlphaClient client;

    public WolframCommandModule(WolframAlphaClient client)
    {
        this.client = client;
    }

    [SlashCommand("ask", "Look something up on Wolfram Alpha")]
    public async Task Search(InteractionContext ctx, [Option("query", "What to ask Wolfram")] string query)
    {
        await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.DeferredChannelMessageWithSource);
        var resultTask = client.ShortAnswerAsync(query);

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(3));
        await Task.WhenAny(resultTask, timeoutTask);

        if (resultTask.IsCompleted)
        {
            var result = resultTask.Result;
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(@$"{Formatter.Bold(query)}:
{result}"));
        }
        else
        {
            // pinging page is taking too long, go ahead and give url then follow up with metadata later
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Still working on it, this is taking longer than usual..."));
            var result = await resultTask;
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent(@$"{Formatter.Bold(query)}:
{result}"));
        }
    }
}
