using ChatBeet.Commands.Discord;
using ChatBeet.Services;
using DSharpPlus.EventArgs;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class IrcLinkValidationRule : IAsyncMessageRule<ModalSubmitEventArgs>
    {
        private readonly IrcMigrationService _migration;

        public IrcLinkValidationRule(IrcMigrationService migration)
        {
            _migration = migration;
        }

        public bool Matches(ModalSubmitEventArgs incomingMessage)=> incomingMessage.Interaction.Data.CustomId == IrcCommandModule.VerifyModalId;

        public async IAsyncEnumerable<IClientMessage> RespondAsync(ModalSubmitEventArgs incomingMessage)
        {
            await _migration.ValidateTokenAsync(incomingMessage.Interaction, incomingMessage.Values["nick"], incomingMessage.Values["token"]);
            yield break;
        }
    }
}
