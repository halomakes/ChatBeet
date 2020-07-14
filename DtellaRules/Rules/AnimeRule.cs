using ChatBeet;
using DtellaRules.Services;
using DtellaRules.Utilities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DtellaRules.Rules
{
    public class AnimeRule : MessageRuleBase<IrcMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly AnilistService client;

        public AnimeRule(IOptions<ChatBeetConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(IrcMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(anime|manga|ln|light novel|ova) (.*)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Content);
            if (match.Success)
            {
                var mediaName = match.Groups[2].Value;
                // use ID instead of name if provided
                var media = await client.GetMediaAsync(mediaName);

                if (media != null)
                {
                    var score = $"{media.Score}%".Colorize(media.Score);

                    yield return new OutboundIrcMessage
                    {
                        Content = $"{IrcValues.BOLD}{media.EnglishTitle}{IrcValues.RESET} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} - {score} • {media.Url}",
                        Target = incomingMessage.Channel
                    };
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Sorry, couldn't find that {match.Groups[1].Value}.",
                        Target = incomingMessage.Channel
                    };
                }
            }
        }
    }
}
