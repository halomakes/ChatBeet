using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public partial class BonkCensorRule : IAsyncMessageRule<MessageReactionAddEventArgs>
    {
        private readonly DiscordClient _discord;

        public BonkCensorRule(DiscordClient discord)
        {
            _discord = discord;
        }

        public bool Matches(MessageReactionAddEventArgs incomingMessage) => incomingMessage.Message.Author == _discord.CurrentUser
            && incomingMessage.Emoji.Name == "bonk";

        [GeneratedRegex(@"^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$")]
        private static partial Regex UrlRgx();

        public async IAsyncEnumerable<IClientMessage> RespondAsync(MessageReactionAddEventArgs incomingMessage)
        {
            await incomingMessage.Message.ModifyEmbedSuppressionAsync(true);
            yield break;
        }
    }
}
