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
                    yield return new OutboundIrcMessage
                    {
                        Content = $"{media.EnglishTitle} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} -  {media.Score}% • {media.Url}",
                        Target = incomingMessage.Channel
                    };

                    yield return new OutboundIrcMessage
                    {
                        Content = media.Description?.StripMarkdown(),
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
