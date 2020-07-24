using ChatBeet;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class AnimeRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ChatBeetConfiguration config;
        private readonly AnilistService client;

        public AnimeRule(IOptions<ChatBeetConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
        }

        public override async IAsyncEnumerable<OutboundIrcMessage> Respond(PrivateMessage incomingMessage)
        {
            var rgx = new Regex($"^{config.CommandPrefix}(anime|manga|ln|light novel|ova) (.*)", RegexOptions.IgnoreCase);
            var match = rgx.Match(incomingMessage.Message);
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
                        Target = incomingMessage.GetResponseTarget()
                    };
                }
                else
                {
                    yield return new OutboundIrcMessage
                    {
                        Content = $"Sorry, couldn't find that {match.Groups[1].Value}.",
                        Target = incomingMessage.GetResponseTarget()
                    };
                }
            }
        }
    }
}
