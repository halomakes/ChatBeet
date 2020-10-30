using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Commands
{
    public class MessageTransformCommandProcessor : NickLookupCommandProcessor
    {
        public MessageTransformCommandProcessor(MessageQueueService messageQueueService, NegativeResponseService negativeResponseService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, negativeResponseService, options) { }
        private static readonly Regex SpacingRegex = new Regex(@"([\x00-\x7F])");

        [Command("kern {nick}")]
        [ChannelOnly]
        public IClientMessage Kern(string nick)
        {
            return Process(nick, lookupMessage => SpacingRegex.Replace(lookupMessage, " $1").Replace("   ", "  ").Trim().ToUpperInvariant());
        }

        [Command("mock {nick}")]
        [ChannelOnly]
        public IClientMessage Mock(string nick)
        {
            var rng = new Random();
            char RandomizeCase(char c) => rng.Next(0, 2) > 0 ? char.ToUpper(c) : char.ToLower(c);

            return Process(nick, lookupMessage => string.Concat(lookupMessage.ToCharArray().Select(RandomizeCase)));
        }

        [Command("colorize {nick}")]
        [ChannelOnly]
        public IClientMessage Colorize(string nick)
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
