using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class AnimeRule : MessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly AnilistService client;

        public AnimeRule(IOptions<IrcBotConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
        }

        public override async IAsyncEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
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

                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"{IrcValues.BOLD}{media.EnglishTitle}{IrcValues.RESET} / {media.RomajiTitle} ({media.NativeTitle}) - {media.Status} - {score} • {media.Url}"
                    );
                }
                else
                {
                    yield return new PrivateMessage(
                        incomingMessage.GetResponseTarget(),
                        $"Sorry, couldn't find that {match.Groups[1].Value}."
                    );
                }
            }
        }
    }
}
