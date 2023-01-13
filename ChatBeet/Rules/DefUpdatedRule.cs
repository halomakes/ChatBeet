using ChatBeet.Configuration;
using ChatBeet.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class DefUpdatedRule : IMessageRule<DefinitionChange>, IAsyncMessageRule<DefinitionChange>
    {
        private readonly IrcBotConfiguration config;
        private readonly DiscordClient _discord;
        private readonly DiscordBotConfiguration _discordConfig;
        private static DiscordChannel channel;

        public DefUpdatedRule(IOptions<IrcBotConfiguration> opts, DiscordClient discord, IOptions<DiscordBotConfiguration> discordOptions)
        {
            config = opts.Value;
            _discord = discord;
            _discordConfig = discordOptions.Value;
        }

        public bool Matches(DefinitionChange incomingMessage) => true;

        public IEnumerable<IClientMessage> Respond(DefinitionChange incomingMessage)
        {
            yield return new PrivateMessage(config.NotifyChannel, $"{IrcValues.BOLD}{incomingMessage.NewNick}{IrcValues.RESET} set {IrcValues.BOLD}{incomingMessage.Key}{IrcValues.RESET} = {incomingMessage.NewValue}");
            if (!string.IsNullOrEmpty(incomingMessage.OldValue))
            {
                yield return new PrivateMessage(config.NotifyChannel, $"Previous value was {IrcValues.BOLD}{incomingMessage.OldValue}{IrcValues.RESET}, set by {incomingMessage.OldNick}.");
            }
        }

        public async IAsyncEnumerable<IClientMessage> RespondAsync(DefinitionChange incomingMessage)
        {
            channel ??= await _discord.GetChannelAsync(_discordConfig.Channels["Audit"]);
            await _discord.SendMessageAsync(channel, $"{Formatter.Bold(incomingMessage.NewNick)} set {Formatter.Bold(incomingMessage.Key)} = {incomingMessage.NewValue}");
            if (!string.IsNullOrEmpty(incomingMessage.OldValue))
            {
                await _discord.SendMessageAsync(channel, $"Previous value was {Formatter.Bold(incomingMessage.OldValue)}, set by {incomingMessage.OldNick}.");
            }
            yield break;
        }
    }
}
