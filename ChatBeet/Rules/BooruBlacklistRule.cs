using ChatBeet.Services;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class BooruBlacklistRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly Regex rgx;
        private readonly BooruService booru;

        public BooruBlacklistRule(IOptions<IrcBotConfiguration> options, BooruService booru)
        {
            config = options.Value;
            rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}booru ((?:whitelist)|(?:blacklist)) ?(.*)", RegexOptions.IgnoreCase);
            this.booru = booru;
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var command = match.Groups[1].Value;
                var tags = match.Groups[2].Value.ToLower().Trim();

                if (string.IsNullOrEmpty(tags))
                {
                    return ListTags(incomingMessage);
                }
                else
                {
                    var tagList = tags.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return command switch
                    {
                        "whitelist" => WhitelistTags(incomingMessage, tagList),
                        "blacklist" => BlacklistTags(incomingMessage, tagList),
                        _ => AsyncEnumerable.Empty<IClientMessage>()
                    };
                }
            }
            return AsyncEnumerable.Empty<IClientMessage>();
        }

        private async IAsyncEnumerable<IClientMessage> ListTags(PrivateMessage message)
        {
            var global = booru.GetGlobalBlacklistedTags();
            yield return new PrivateMessage(message.From, $"{IrcValues.BOLD}Global blacklist{IrcValues.RESET}: [{string.Join(", ", global)}]");
            var user = await booru.GetBlacklistedTags(message.From);
            yield return new PrivateMessage(message.From, $"{IrcValues.BOLD}User blacklist{IrcValues.RESET}: [{string.Join(", ", user)}]");
            yield return new PrivateMessage(message.From, $"Use {IrcValues.ITALIC}{config.CommandPrefix}booru blacklist/whitelist [tags]{IrcValues.RESET} to manage your personal blacklist.");
        }

        private async IAsyncEnumerable<IClientMessage> WhitelistTags(PrivateMessage message, IEnumerable<string> tags)
        {
            await booru.WhitelistTags(message.From, tags);
            yield return new PrivateMessage(message.From, $"[{string.Join(", ", tags)}] removed from your blacklist.");
        }

        private async IAsyncEnumerable<IClientMessage> BlacklistTags(PrivateMessage message, IEnumerable<string> tags)
        {
            await booru.BlacklistTags(message.From, tags);
            yield return new PrivateMessage(message.From, $"[{string.Join(", ", tags)}] added to your blacklist.");
        }
    }
}