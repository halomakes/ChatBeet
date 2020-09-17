using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class KerningRule : NickLookupRule
    {
        private static readonly Regex rgx = new Regex(@"([\x00-\x7F])");

        public KerningRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options, NegativeResponseService nrService) : base(messageQueueService, options, nrService)
        {
            CommandName = "kern";
        }

        protected override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var spaced = rgx.Replace(lookupMessage.Message, " $1").Replace("   ", "  ").Trim().ToUpperInvariant();

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {spaced}");
        }
    }
}