using ChatBeet;
using DtellaRules.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class AutoYatoRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly string autoYatoUrl;

        public AutoYatoRule(IOptions<DtellaRuleConfiguration> dtellaOptions, IOptions<ChatBeetConfiguration> options)
        {
            autoYatoUrl = dtellaOptions.Value.Urls["AutoYato"];
            config = options.Value;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($@"^{config.BotName}, what does yato think (?:about|of) ([^\?]*)\??", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Message))
            {
                var topic = rgx.Replace(incomingMessage.Message, @"$1");
                var url = $"{autoYatoUrl}/{WebUtility.UrlEncode(topic)}";

                yield return new OutboundIrcMessage
                {
                    Content = $"Here's what yato thinks of {topic}: {url}",
                    OutputType = IrcMessageType.Message,
                    Target = incomingMessage.GetResponseTarget()
                };
            }
        }
    }
}
