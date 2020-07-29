using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class DeviantartRule : MessageRuleBase<PrivateMessage>
    {
        private readonly DeviantartService daService;
        private readonly IrcBotConfiguration config;

        public DeviantartRule(DeviantartService daService, IOptions<IrcBotConfiguration> options)
        {
            this.daService = daService;
            config = options.Value;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(da|deviantart|degenerate) (.*)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var search = match.Groups[2].Value;
                // use ID instead of name if provided
                var media = await daService.GetRecentImageAsync(search);

                if (media != null)
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{IrcValues.BOLD}{media.Title?.Text}{IrcValues.RESET} - {media.Id}");
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything matching {match.Groups[2].Value}.");
                }
            }
        }
    }
}
