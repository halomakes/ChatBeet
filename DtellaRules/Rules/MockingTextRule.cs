using ChatBeet;
using ChatBeet.Irc;
using DtellaRules.Utilities;
using GravyIrc.Messages;
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

        protected override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var rng = new Random();
            var stupidifyalized = string.Concat(lookupMessage.Message.ToCharArray().Select(c => GetCase() ? char.ToUpper(c) : char.ToLower(c)));

            yield return new OutboundIrcMessage
            {
                Content = $"<{lookupMessage.From}> {stupidifyalized}",
                Target = incomingMessage.GetResponseTarget()
            };

            bool GetCase() => rng.Next(0, 2) > 0;
        }
    }
}