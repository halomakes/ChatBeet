﻿using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public partial class KarmaReactRule : IMessageRule<PrivateMessage>, IAsyncMessageRule<MessageCreateEventArgs>
    {
        private readonly Regex filter;
        private static DateTime? lastReactionTime = null;
        private static string lastReaction = null;
        private static readonly TimeSpan debounce = TimeSpan.FromSeconds(20);
        private readonly DiscordClient _discord;

        public KarmaReactRule(IOptions<IrcBotConfiguration> options, DiscordClient discord)
        {
            filter = new Regex($@"^{Regex.Escape(options.Value.Nick)}((\+\+)|(--))$", RegexOptions.IgnoreCase);
            _discord = discord;
        }

        public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var match = filter.Match(incomingMessage.Message);
            if (match.Success)
            {
                var reaction = match.Groups[1].Value switch
                {
                    "++" => "yee",
                    "--" => "fak",
                    _ => default
                };

                if (!string.IsNullOrEmpty(reaction))
                {
                    if (reaction == lastReaction)
                    {
                        if (!lastReactionTime.HasValue || (DateTime.Now - lastReactionTime.Value) > debounce)
                        {
                            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), reaction);
                        }
                    }
                    else
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), reaction);
                    }
                    lastReaction = reaction;
                    lastReactionTime = DateTime.Now;
                }
            }
        }

        public async IAsyncEnumerable<IClientMessage> RespondAsync(MessageCreateEventArgs incomingMessage)
        {
            await incomingMessage.Message.RespondAsync("yee");
            await incomingMessage.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("😃"));
            yield break;
        }

        public bool Matches(MessageCreateEventArgs incomingMessage) => filter.IsMatch(incomingMessage.Message.Content) || IsDiscordUpvode(incomingMessage.Message);

        [GeneratedRegex(@"^\<@\d+\> ?\+\+$", RegexOptions.IgnoreCase)]
        private partial Regex discordRgx();

        private bool IsDiscordUpvode(DiscordMessage message) =>
            (message.MessageType == MessageType.Default || message.MessageType == MessageType.Reply)
            && message.MentionedUsers.Count() == 1
            && message.MentionedUsers.Single() == _discord.CurrentUser
            && discordRgx().IsMatch(message.Content);
    }
}
