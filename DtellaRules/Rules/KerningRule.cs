using ChatBeet;
using ChatBeet.Irc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace DtellaRules.Rules
{
    public class KerningRule : NickLookupRule
    {
        public KerningRule(MessageQueueService messageQueueService, IOptions<ChatBeetConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "kern";
        }

        protected override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage, string nick, IrcMessage lookupMessage)
        {
            var spaced = string.Join(" ", lookupMessage.Content.ToCharArray()).ToUpper();

            yield return new OutboundIrcMessage
            {
                Content = $"<{nick}> {spaced}",
                Target = incomingMessage.Channel
            };
        }
    }
}