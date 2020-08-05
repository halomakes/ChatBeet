using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class GifSearchRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly TenorGifService gifService;
        private readonly Regex rgx;
        private readonly IrcBotConfiguration config;

        public GifSearchRule(TenorGifService gifService, IOptions<IrcBotConfiguration> options)
        {
            config = options.Value;
            this.gifService = gifService;
            rgx = new Regex($"^{config.CommandPrefix}(gif) (.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var search = match.Groups[2].Value;

                var url = await gifService.GetGifAsync(search);

                if (!string.IsNullOrEmpty(url))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{url} - {IrcValues.AQUA}Via Tenor");
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Sorry, couldn't find that anything for {IrcValues.ITALIC}{search.Trim()}{IrcValues.RESET}.");
                }
            }
        }
    }
}
