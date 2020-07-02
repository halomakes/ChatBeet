using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.DefaultRules.Rules
{
    public class ExceptionLoggingRule : MessageRuleBase<ExceptionMessage>, IMessageRule<ExceptionMessage>
    {
        private readonly DefaultRulesConfiguration config;

        public ExceptionLoggingRule(IOptions<DefaultRulesConfiguration> options)
        {
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(ExceptionMessage incomingMessage)
        {
            yield return new OutboundIrcMessage
            {
                Content = $"Encountered exception from {incomingMessage.Sender}",
                OutputType = IrcMessageType.Announcement,
                Target = config.LogChannel
            };

            yield return new OutboundIrcMessage
            {
                Content = $"Base exception: {incomingMessage.Exception.Message}",
                Target = config.LogChannel
            };

            var depth = 0;
            var currentException = incomingMessage.Exception;

            while (depth < 4 && currentException.InnerException != null)
            {
                currentException = currentException.InnerException;
                depth++;

                yield return new OutboundIrcMessage
                {
                    Content = $"Inner exception: {currentException.Message}",
                    Target = config.LogChannel
                };
            }
        }
    }
}
