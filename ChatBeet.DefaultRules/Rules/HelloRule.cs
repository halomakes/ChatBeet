using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.DefaultRules.Rules
{
    public class HelloRule : MessageRuleBase<IrcMessage>, IMessageRule<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;

        public HelloRule(IOptions<ChatBeetConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            if (incomingMessage.Content == $"{config.CommandPrefix}hello")
            {
                yield return new OutboundIrcMessage
                {
                    Content = $"Hello, {incomingMessage.Sender}!",
                    OutputType = IrcMessageType.Message,
                    Target = incomingMessage.Channel
                };
            }
        }
    }
}
