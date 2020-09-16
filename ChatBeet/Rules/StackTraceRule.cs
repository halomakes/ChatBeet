using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Rules
{
    public class StackTraceRule : MessageRuleBase<Exception>
    {
        private readonly IrcBotConfiguration config;

        public StackTraceRule(IOptions<IrcBotConfiguration> opts)
        {
            config = opts.Value;
        }

        public override IEnumerable<IClientMessage> Respond(Exception incomingMessage) => incomingMessage.StackTrace
            .Replace("\r\n", "\n").Split("\n")
            .Select(line => new PrivateMessage(config.LogChannel, $"{IrcValues.YELLOW}{line}{IrcValues.RESET}"));
    }
}
