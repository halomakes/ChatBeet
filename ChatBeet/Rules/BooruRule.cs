using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class BooruRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly Regex rgx;
        private readonly BooruService booru;

        public BooruRule(IOptions<IrcBotConfiguration> options, BooruService booru)
        {
            config = options.Value;
            rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}((booru)|(nsfwbooru))(?!(?: blacklist)|(?: whitelist)) (.*)", RegexOptions.IgnoreCase);
            this.booru = booru;
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var command = match.Groups[1].Value.Trim();
                var tagList = match.Groups[4].Value.Trim();
                var tags = tagList.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var applyFilter = command != "nsfwbooru";

                if (tags.Any())
                {
                    var text = await booru.GetRandomPostAsync(applyFilter, incomingMessage.From, tags);

                    if (text != default)
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), text);
                        await booru.RecordTags(incomingMessage.From, tags);
                    }
                    else
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything for {tagList}, ya perv. See available tags here: https://gelbooru.com/index.php?page=tags&s=list");
                    }
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Please specify tag/tags. See available list here: https://gelbooru.com/index.php?page=tags&s=list");
                }
            }
        }
    }
}