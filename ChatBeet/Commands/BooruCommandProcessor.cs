using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Commands
{
    public class BooruCommandProcessor : CommandProcessor
    {
        private readonly IrcBotConfiguration config;
        private readonly BooruService booru;

        public BooruCommandProcessor(IOptions<IrcBotConfiguration> options, BooruService booru)
        {
            config = options.Value;
            this.booru = booru;
        }

        [Command("booru {tagList}", Description = "Get a random image from gelbooru matching tags (safe only).")]
        [Command("nsfwbooru {tagList}", Description = "Get a random image from gelbooru matching tags (questionable and explicit only).")]
        [ChannelPolicy("NoMain")]
        public async IAsyncEnumerable<IClientMessage> GetRandomPost(string tagList)
        {
            var tags = tagList.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var applyFilter = TriggeringCommandName != "nsfwbooru";

            if (tags.Any())
            {
                var text = await booru.GetRandomPostAsync(applyFilter, IncomingMessage.From, tags);

                if (text != default)
                {
                    await booru.RecordTags(IncomingMessage.From, tags);
                    yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), text);
                }
                else
                {
                    yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything for {tagList}, ya perv. See available tags here: https://gelbooru.com/index.php?page=tags&s=list");
                }
            }
            else
            {
                yield return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Please specify tag/tags. See available list here: https://gelbooru.com/index.php?page=tags&s=list");
            }
        }

        [Command("booru whitelist {tagList}", Description = "Remove tag(s) from your blacklist.")]
        [Command("booru blacklist {tagList}", Description = "Add tag(s) to your blacklist.")]
        public IAsyncEnumerable<IClientMessage> HandleBlacklistCommand(string tagList)
        {
            var tags = tagList?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (tags == default || !tags.Any())
                return ListTags();
            return TriggeringCommandName switch
            {
                "booru whitelist" => WhitelistTags(tags),
                "booru blacklist" => BlacklistTags(tags),
                _ => AsyncEnumerable.Empty<IClientMessage>()
            };
        }

        private async IAsyncEnumerable<IClientMessage> ListTags()
        {
            var global = booru.GetGlobalBlacklistedTags();
            yield return new PrivateMessage(IncomingMessage.From, $"{IrcValues.BOLD}Global blacklist{IrcValues.RESET}: [{string.Join(", ", global)}]");
            var user = await booru.GetBlacklistedTags(IncomingMessage.From);
            yield return new PrivateMessage(IncomingMessage.From, $"{IrcValues.BOLD}User blacklist{IrcValues.RESET}: [{string.Join(", ", user)}]");
            yield return new PrivateMessage(IncomingMessage.From, $"Use {IrcValues.ITALIC}{config.CommandPrefix}booru blacklist/whitelist [tags]{IrcValues.RESET} to manage your personal blacklist.");
        }

        private async IAsyncEnumerable<IClientMessage> WhitelistTags(IEnumerable<string> tags)
        {
            await booru.WhitelistTags(IncomingMessage.From, tags);
            yield return new PrivateMessage(IncomingMessage.From, $"[{string.Join(", ", tags)}] removed from your blacklist.");
        }

        private async IAsyncEnumerable<IClientMessage> BlacklistTags(IEnumerable<string> tags)
        {
            await booru.BlacklistTags(IncomingMessage.From, tags);
            yield return new PrivateMessage(IncomingMessage.From, $"[{string.Join(", ", tags)}] added to your blacklist.");
        }
    }
}
