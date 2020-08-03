using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class KerningRule : NickLookupRule
    {
        public KerningRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "kern";
        }

        protected override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var spaced = string.Join(" ", lookupMessage.Message.ToCharArray()).ToUpper();

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {spaced}");
        }
    }
}