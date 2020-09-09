using ChatBeet.Models;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class DefUpdatedRule : MessageRuleBase<DefinitionChange>
    {
        private readonly IrcBotConfiguration config;

        public DefUpdatedRule(IOptions<IrcBotConfiguration> opts)
        {
            config = opts.Value;
        }

        public override IEnumerable<IClientMessage> Respond(DefinitionChange incomingMessage)
        {
            yield return new PrivateMessage(config.NotifyChannel, $"{IrcValues.BOLD}{incomingMessage.NewNick}{IrcValues.RESET} set {IrcValues.BOLD}{incomingMessage.Key}{IrcValues.RESET} = {incomingMessage.NewValue}");
            if (!string.IsNullOrEmpty(incomingMessage.OldValue))
            {
                yield return new PrivateMessage(config.NotifyChannel, $"Previous value was {IrcValues.BOLD}{incomingMessage.OldValue}{IrcValues.RESET}, set by {incomingMessage.OldNick}.");
            }
        }
    }
}
