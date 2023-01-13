using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Humanizer;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashCommandGroup("system", "Commands about system information")]
public class SystemInfoCommandModule : ApplicationCommandModule
{
    [SlashCommand("version", "Check which version of the bot is running")]
    public async Task GetVersion(InteractionContext ctx) => await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"Running {GetName()}")
            );

    [SlashCommand("uptime", "Check how long the bot has been online")]
    public async Task GetUptime(InteractionContext ctx) => await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"Online for {(DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize()}")
            );

    [SlashCommand("host", "Get information about the bot's host environment")]
    public async Task GetHostInfo(InteractionContext ctx) => await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"Running on {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture.ToString().ToLower()} with {RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture.ToString().ToLower()}")
            );
    private AssemblyName GetName() => Assembly.GetExecutingAssembly().GetName();
}
