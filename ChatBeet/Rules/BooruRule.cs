using BooruSharp.Booru;
using BooruSharp.Search.Post;
using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class BooruRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly ChatBeetConfiguration.PixivConfiguration pixivConfig;
        private readonly IMemoryCache cache;
        private readonly Regex rgx;
        private readonly Gelbooru gelbooru;

        public BooruRule(IOptions<IrcBotConfiguration> options, IOptions<ChatBeetConfiguration> dtlaOptions, IMemoryCache cache, Gelbooru gelbooru)
        {
            config = options.Value;
            pixivConfig = dtlaOptions.Value.Pixiv;
            this.cache = cache;
            this.gelbooru = gelbooru;
            rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}((booru)|(nsfwbooru)) (.*)", RegexOptions.IgnoreCase);
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
                var filter = command switch
                {
                    "nsfwbooru" => "-rating:safe",
                    _ => "rating:safe"
                };

                if (tags.Any())
                {
                    var results = await cache.GetOrCreateAsync($"{command}:{tagList}", async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                        return await gelbooru.GetRandomPostsAsync(20, tags.Append(filter).ToArray());
                    });

                    var text = PickImage(results, tags);
                    if (text != null)
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), text);
                    }
                    else
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Sorry, couldn't find anything for {tagList}, ya perv.");
                    }


                    static string PickImage(IEnumerable<SearchResult> searchResults, IEnumerable<string> searchTags)
                    {
                        if (searchResults?.Any() ?? false)
                        {
                            var img = searchResults.PickRandom();
                            var rng = new Random();
                            var resultTags = img.tags
                                .Select(t => (MatchesInput: searchTags.Contains(t), Tag: t))
                                .OrderByDescending(p => p.MatchesInput)
                                .ThenBy(p => rng.Next())
                                .Select(p => p.MatchesInput ? $"{IrcValues.BOLD}{IrcValues.GREEN}{p.Tag}{IrcValues.RESET}" : p.Tag)
                                .Take(10)
                                .OrderBy(t => rng.Next());
                            var tagList = string.Join(", ", resultTags);
                            return $"{img.fileUrl} - {tagList}";
                        }
                        return null;
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