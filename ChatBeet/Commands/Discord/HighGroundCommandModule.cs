﻿using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

public class HighGroundCommandModule : ApplicationCommandModule
{
    public static readonly Dictionary<DiscordGuild, DiscordUser> HighestUsers = new();
    private static readonly Dictionary<DiscordUser, DateTime> InvocationHistory = new();
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(5);
    private readonly GraphicsService _graphics;

    public HighGroundCommandModule(GraphicsService graphics)
    {
        _graphics = graphics;
    }

    [SlashCommand("jump", "Claim the high ground")]
    public async Task Claim(InteractionContext ctx)
    {
        var server = ctx.Channel.Guild;
        var user = ctx.User;

        if (InvocationHistory.TryGetValue(user, out var lastActivation) && (DateTime.Now - lastActivation) < Timeout)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Shouldn't have skipped leg day.  You will be ready to jump again {Formatter.Timestamp(lastActivation + Timeout)}"));
            return;
        }

        if (!HighestUsers.ContainsKey(server))
        {
            HighestUsers[server] = user;
            using var graphic = await _graphics.BuildHighGroundImageAsync($"#{ctx.Channel.Name}", user.Username);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} has the high ground.")
                .AddFile("high-ground.webp", graphic));
            return;
        }
        else if (user == HighestUsers[server])
        {
            HighestUsers.Remove(server);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(user)} trips and falls off the high ground."));
            return;
        }
        else
        {
            var oldKing = HighestUsers[server];
            HighestUsers[server] = user;
            var graphic = await _graphics.BuildHighGroundImageAsync(oldKing.Username, user.Username);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"It's over, {Formatter.Mention(oldKing)}! {Formatter.Mention(user)} has the high ground!")
                .AddFile("high-ground.webp", graphic));
            return;
        }
    }
}
