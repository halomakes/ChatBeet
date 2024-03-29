﻿using System.Threading.Tasks;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

public class EightBallCommandModule : ApplicationCommandModule
{
    [SlashCommand("8-ball", "Get an 8-ball response")]
    public async Task BeHurt(InteractionContext ctx, [Option("question", "Question to ask the mystical 8-ball")] string question)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($@"{Formatter.Bold(question)}: {YesNoGenerator.GetResponse()}")
                );
    }
}
