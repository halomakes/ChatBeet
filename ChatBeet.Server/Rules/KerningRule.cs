using ChatBeet;
using ChatBeet.Irc;
using ChatBeet.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class KerningRule : NickLookupRule
    {
        public KerningRule(MessageQueueService messageQueueService, IOptions<ChatBeetConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "kern";
        }

        protected override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var spaced = string.Join(" ", lookupMessage.Message.ToCharArray()).ToUpper();

            yield return new OutboundIrcMessage
            {
                Content = $"<{lookupMessage.From}> {spaced}",
                Target = incomingMessage.GetResponseTarget()
            };
        }
    }
}