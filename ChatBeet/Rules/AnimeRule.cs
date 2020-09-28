using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using Miki.Anilist;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class AnimeRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly AnilistService client;
        private readonly Regex filter;

        public AnimeRule(IOptions<IrcBotConfiguration> options, AnilistService client)
        {
            config = options.Value;
            this.client = client;
            filter = new Regex($"^{Regex.Escape(config.CommandPrefix)}(anime|manga|ln|light novel|ova) (.*)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => filter.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                var type = match.Groups[1].Value switch
                {
                    "anime" => MediaType.ANIME,
                    "manga" => MediaType.MANGA,
                    "ln" => MediaType.MANGA,
                    "light novel" => MediaType.MANGA,
                    "ova" => MediaType.ANIME,
                    _ => MediaType.ANIME
                };
                var mediaName = match.Groups[2].Value;
                // use ID instead of name if provided
                var media = await client.GetMediaAsync(mediaName, type);

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
