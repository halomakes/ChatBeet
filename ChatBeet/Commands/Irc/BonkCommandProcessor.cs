using ChatBeet.Attributes;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ChatBeet.Commands.Irc;

public class BonkCommandProcessor : CommandProcessor
{
    private readonly IrcBotConfiguration config;

    public BonkCommandProcessor(IOptions<IrcBotConfiguration> options)
    {
        config = options.Value;
    }

    [Command("bonk {nick}", Description = "Call someone out for being horni")]
    [ChannelOnly]
    public IClientMessage Bonk([Nick, Required] string nick) =>
        new PrivateMessage(IncomingMessage.GetResponseTarget(), $"🚨🚨 {nick} has been reported as being horny 🚨🚨  {config.Nick} is now contacting the {IrcValues.RED}FBI{IrcValues.RESET}, {IrcValues.WHITE}NSA{IrcValues.RESET}, {IrcValues.BLUE}CIA{IrcValues.RESET}, {IrcValues.LIME}Navy SEALs{IrcValues.RESET}, {IrcValues.TEAL}Secret Service{IrcValues.RESET}, and {IrcValues.PINK}ur mom{IrcValues.RESET}. ");
}