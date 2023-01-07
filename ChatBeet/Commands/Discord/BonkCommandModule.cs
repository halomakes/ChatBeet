using ChatBeet.Attributes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class BonkCommandModule : ApplicationCommandModule
{
    private readonly IrcBotConfiguration config;

    public BonkCommandModule(IOptions<IrcBotConfiguration> options)
    {
        config = options.Value;
    }

    [SlashCommand("bonk", "Call someone out for being horni")]
    public async Task Bonk(InteractionContext ctx, [Option("user", "Person to compliment")] DiscordUser user)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"🚨🚨 {Formatter.Mention(user)} has been reported as being horny 🚨🚨  {config.Nick} is now contacting the {Formatter.Bold("FBI")}, {Formatter.Bold("NSA")}, {Formatter.Bold("CIA")}, {Formatter.Bold("Navy SEALs")}, {Formatter.Bold("Secret Service")}, and {Formatter.Bold("ur mom")}. ")
                );
    }
}
