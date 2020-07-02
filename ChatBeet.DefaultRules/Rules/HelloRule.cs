using System.Collections.Generic;

namespace ChatBeet.DefaultRules.Rules
{
    public class HelloRule : MessageRuleBase<IrcMessage>, IMessageRule<IrcMessage>
    {
        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            if (incomingMessage.Content == "🥕 hello")
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
