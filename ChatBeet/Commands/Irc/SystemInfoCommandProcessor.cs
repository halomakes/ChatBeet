using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using Humanizer;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ChatBeet.Commands.Irc;

public class SystemInfoCommandProcessor : CommandProcessor
{
    [Command("version", Description = "Check which version of the bot is running.")]
    public IClientMessage GetVersion() =>
        new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Running {GetName()}");

    [Command("uptime", Description = "Check how long the bot has been online.")]
    public IClientMessage GetUptime() =>
        new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Online for {(DateTime.Now - Process.GetCurrentProcess().StartTime).Humanize()}");

    [Command("host", Description = "Get information about the bot's host environment.")]
    public IClientMessage GetHostInfo() =>
        new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Running on {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture.ToString().ToLower()} with {RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture.ToString().ToLower()}");

    private AssemblyName GetName() => Assembly.GetExecutingAssembly().GetName();
}