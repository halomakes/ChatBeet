using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class ShipReactRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly MessageQueueService messageQueueService;
        private readonly Regex responseRgx = new Regex(@"^([|\-A-z0-9]*): ([^{}]+) {([A-z0-9_\-\(\), ]+)}", RegexOptions.IgnoreCase);
        private readonly BooruService booru;

        public ShipReactRule(BooruService booru, MessageQueueService messageQueueService)
        {
            this.booru = booru;
            this.messageQueueService = messageQueueService;
        }

        public override bool Matches(PrivateMessage incomingMessage) => incomingMessage.From == "vanvan" && responseRgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = responseRgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var requestor = match.Groups[1].Value;
                var ship = match.Groups[2].Value;
                var tags = match.Groups[3].Value.Split(",").Select(s => s.Trim().ToLowerInvariant());

                if (tags.Any())
                {
                    var commandMessage = messageQueueService.GetLatestMessage(requestor, incomingMessage.To);

                    if (commandMessage.Message.StartsWith(".shipp @b "))
                    {
                        var text = await booru.GetRandomPostAsync(null, requestor, tags);

                        if (text != default)
                        {
                            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"An example of {IrcValues.BOLD}{ship}{IrcValues.RESET}: {text}");
                        }
                    }
                }
            }
        }
    }
}