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
using System.Text.RegularExpressions;

namespace ChatBeet.Commands
{
    public class MessageTransformCommandProcessor : NickLookupCommandProcessor
    {
        public MessageTransformCommandProcessor(MessageQueueService messageQueueService, NegativeResponseService negativeResponseService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, negativeResponseService, options) { }
        private static readonly Regex SpacingRegex = new Regex(@"([\x00-\x7F])");

        [Command("kern {nick}", Description = "Make someone's text uppercase and space it out.")]
        [ChannelOnly]
        public IClientMessage Kern(
            [Required, RegularExpression(@"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+", ErrorMessage = "Enter a valid IRC nick.")] string nick
            )
        {
            return Process(nick, lookupMessage => SpacingRegex.Replace(lookupMessage, " $1").Replace("   ", "  ").Trim().ToUpperInvariant());
        }

        [Command("mock {nick}", Description = "Apply random casing to each letter in someone's message.")]
        [ChannelOnly]
        public IClientMessage Mock(
            [Required, RegularExpression(@"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+", ErrorMessage = "Enter a valid IRC nick.")] string nick
            )
        {
            return Process(nick, lookupMessage => string.Concat(RandomizeCase(lookupMessage)));

            static IEnumerable<char> RandomizeCase(string s)
            {
                var rng = new Random();
                var switchProbability = .8; // 80% chance to change case each character
                var isUppercase = rng.Next(0, 2) > 0; // 50% probability for first character
                foreach (var @char in s)
                {
                    if (rng.NextDouble() < switchProbability)
                        isUppercase = !isUppercase;

                    yield return isUppercase ? char.ToUpper(@char) : char.ToLower(@char);
                }
            }
        }

        [Command("colorize {nick}", Description = "Apply random colors to someone's message")]
        [ChannelOnly]
        public IClientMessage Colorize(
            [Required, RegularExpression(@"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+", ErrorMessage = "Enter a valid IRC nick.")] string nick
            )
        {
            var colors = new List<string> { IrcValues.AQUA, IrcValues.BLUE, IrcValues.BROWN, IrcValues.GREEN, IrcValues.LIME, IrcValues.ORANGE, IrcValues.PINK, IrcValues.PURPLE, IrcValues.RED, IrcValues.ROYAL, IrcValues.YELLOW };
            return Process(nick, lookupMessage =>
            {
                var segments = SpacingRegex.Split(lookupMessage);
                var colorSequence = Enumerable.Range(0, segments.Length).Select(_ => colors.PickRandom());
                var joined = segments.Zip(colorSequence).SelectMany(pair => new string[] { pair.First, pair.Second });
                return string.Join(string.Empty, joined).Trim();
            });
        }

        private IClientMessage Process(string nick, Func<string, string> transformer) =>
            Process(nick, lookupMessage => new PrivateMessage(IncomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {transformer(lookupMessage.Message)}"));
    }
}
