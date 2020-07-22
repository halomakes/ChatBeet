using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.DefaultRules.Rules
{
    public class HelloRule : MessageRuleBase<PrivateMessage>, IMessageRule<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;

        public HelloRule(IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage)
        {
            if (incomingMessage.Message == $"{config.CommandPrefix}hello")
            {
                yield return new OutboundIrcMessage
                {
                    Content = $"Hello, {incomingMessage.From}!",
                    OutputType = IrcMessageType.Message,
                    Target = incomingMessage.To
                };
            }
        }
    }
}
