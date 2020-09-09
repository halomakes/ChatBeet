using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class BadBotReactRule : MessageRuleBase<PrivateMessage>
    {
        private readonly Regex filter;
        private static DateTime? lastReactionTime = null;
        private static string lastReaction = null;
        private static readonly TimeSpan debounce = TimeSpan.FromSeconds(20);

        public BadBotReactRule(IOptions<IrcBotConfiguration> options)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.CommandPrefix)}(bad|shit) bot$", RegexOptions.IgnoreCase);
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (filter.IsMatch(incomingMessage.Message))
            {
                if (!lastReactionTime.HasValue || (DateTime.Now - lastReactionTime.Value) > debounce)
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), "*sad bot noises*");
                }
                lastReactionTime = DateTime.Now;
            }
        }
    }
}
