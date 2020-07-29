using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Rules
{
    public class MockingTextRule : NickLookupRule
    {
        public MockingTextRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "mock";
        }

        protected override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var rng = new Random();
            var stupefied = string.Concat(lookupMessage.Message.ToCharArray().Select(RandomizeCase));

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {stupefied}");

            char RandomizeCase(char c) => rng.Next(0, 2) > 0 ? char.ToUpper(c) : char.ToLower(c);
        }
    }
}