using ChatBeet.Models;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class HighGroundRule : IMessageRule<PrivateMessage>, IMessageRule<HighGroundClaim>
    {
        private readonly Regex filter;
        public static readonly Dictionary<string, string> HighestNicks = new Dictionary<string, string>();

        public HighGroundRule(IOptions<IrcBotConfiguration> options)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.CommandPrefix)}((climb)|(jump)|(high ground))$", RegexOptions.IgnoreCase);
        }

        public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            if (filter.IsMatch(incomingMessage.Message))
            {
                if (incomingMessage.IsChannelMessage)
                {
                    yield return GetResponse(incomingMessage.From, incomingMessage.To, incomingMessage.GetResponseTarget());
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.From, $"You must be in a channel to claim the high ground.");
                }
            }
        }

        public IEnumerable<IClientMessage> Respond(HighGroundClaim incomingMessage)
        {
            yield return GetResponse(incomingMessage.Nick, incomingMessage.Channel, incomingMessage.Channel);
        }

        private PrivateMessage GetResponse(string nick, string chan, string target)
        {
            if (!HighestNicks.ContainsKey(chan))
            {
                HighestNicks[chan] = nick;
                return new PrivateMessage(target, $"{nick} has the high ground.");
            }
            else if (nick == HighestNicks[chan])
            {
                HighestNicks.Remove(chan);
                return new PrivateMessage(target, $"{nick} trips and falls off the high ground.");
            }
            else
            {
                var oldKing = HighestNicks[chan];
                HighestNicks[chan] = nick;
                return new PrivateMessage(target, $"It's over, {oldKing}! {nick} has the high ground!");
            }
        }

        public IEnumerable<IClientMessage> Respond(object incomingMessage) => incomingMessage switch
        {
            PrivateMessage pm => Respond(pm),
            HighGroundClaim hgc => Respond(hgc),
            _ => Enumerable.Empty<IClientMessage>()
        };
    }
}
