using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        [RateLimit(30, TimeUnit.Second)]
        public async Task<IClientMessage> GetRandomSafePost([Required] string tagList) => await GetPost(true, tagList);

        [Command("nsfwbooru {tagList}", Description = "Get a random image from gelbooru matching tags (questionable and explicit only).")]
        [ChannelPolicy("NoMain")]
        public async Task<IClientMessage> GetRandomPost([Required] string tagList) => await GetPost(false, tagList);

        private async Task<IClientMessage> GetPost(bool safeOnly, string tagList)
        {
            var tags = tagList.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tags.Any())
            {
                var text = await booru.GetRandomPostAsync(safeOnly, IncomingMessage.From, tags);

                if (text is not null)
                {
                    await booru.RecordTags(IncomingMessage.From, tags);
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), text);
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything for {tagList}, ya perv. See available tags here: https://gelbooru.com/index.php?page=tags&s=list");
                }
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Please specify tag/tags. See available list here: https://gelbooru.com/index.php?page=tags&s=list");
            }
        }

        [Command("booru whitelist {tagList}", Description = "Remove tag(s) from your blacklist.")]
        [Command("booru blacklist {tagList}", Description = "Add tag(s) to your blacklist.")]
        public IAsyncEnumerable<IClientMessage> HandleBlacklistCommand([Required] string tagList)
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

        [Command("astolfo", Description = "Fill the void in your soul with an Astolfo picture."), RateLimit(30, TimeUnit.Second)]
        public async Task<IClientMessage> GetAstolfo()
        {
            var text = await booru.GetRandomPostAsync(true, IncomingMessage.From, "astolfo_(fate)");
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), text ?? "Sorry, couldn't find locate those succulent thighs.");
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
