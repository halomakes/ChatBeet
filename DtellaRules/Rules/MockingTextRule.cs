using ChatBeet;
using ChatBeet.Irc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DtellaRules.Rules
{
    public class MockingTextRule : NickLookupRule
    {
        public MockingTextRule(MessageQueueService messageQueueService, IOptions<ChatBeetConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "mock";
        }

        protected override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage, string nick, IrcMessage lookupMessage)
        {
            var rng = new Random();
            var stupidifyalized = string.Concat(lookupMessage.Content.ToCharArray().Select(c => GetCase() ? char.ToUpper(c) : char.ToLower(c)));

            yield return new OutboundIrcMessage
            {
                Content = $"<{lookupMessage.Sender}> {stupidifyalized}",
                Target = incomingMessage.Channel
            };

            bool GetCase() => rng.Next(0, 2) > 0;
        }
    }
}