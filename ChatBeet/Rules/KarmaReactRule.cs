using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class KarmaReactRule : MessageRuleBase<PrivateMessage>
    {
        private readonly Regex filter;
        private static DateTime? lastReactionTime = null;
        private static string lastReaction = null;
        private static readonly TimeSpan debounce = TimeSpan.FromSeconds(20);

        public KarmaReactRule(IOptions<IrcBotConfiguration> options)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.Nick)}((\+\+)|(--))$", RegexOptions.IgnoreCase);
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                var reaction = match.Groups[1].Value switch
                {
                    "++" => "yee",
                    "--" => "fak",
                    _ => default
                };

                if (!string.IsNullOrEmpty(reaction))
                {
                    if (reaction == lastReaction)
                    {
                        if (!lastReactionTime.HasValue || (DateTime.Now - lastReactionTime.Value) > debounce)
                        {
                            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), reaction);
                        }
                    }
                    else
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), reaction);
                    }
                    lastReaction = reaction;
                    lastReactionTime = DateTime.Now;
                }
            }
        }
    }
}
