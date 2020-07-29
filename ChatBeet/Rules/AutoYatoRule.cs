using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class AutoYatoRule : MessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly string autoYatoUrl;

        public AutoYatoRule(IOptions<DtellaRuleConfiguration> dtellaOptions, IOptions<IrcBotConfiguration> options)
        {
            autoYatoUrl = dtellaOptions.Value.Urls["AutoYato"];
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($@"^{config.Nick}, what does yato think (?:about|of) ([^\?]*)\??", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(incomingMessage.Message))
            {
                var topic = rgx.Replace(incomingMessage.Message, @"$1");
                var url = $"{autoYatoUrl}/{WebUtility.UrlEncode(topic)}";

                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Here's what yato thinks of {topic}: {url}");
            }
        }
    }
}
