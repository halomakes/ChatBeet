using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class PronounRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly UserPreferencesService userPreferences;
        private readonly IrcBotConfiguration config;
        private readonly Regex rgx;

        public PronounRule(UserPreferencesService userPreferences, IOptions<IrcBotConfiguration> options)
        {
            this.userPreferences = userPreferences;
            config = options.Value;
            rgx = new Regex($@"^{Regex.Escape(config.CommandPrefix)}pronouns ([^\ ]*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var nick = match.Groups[1].Value;

                if (nick.Equals(config.Nick, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From}: no u");
                }
                else
                {
                    var subject = await userPreferences.Get(nick, UserPreference.SubjectPronoun);
                    var @object = await userPreferences.Get(nick, UserPreference.ObjectPronoun);
                    if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(@object))
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Sorry, I don't know the preferred pronouns for {nick}.");
                    }
                    else if (string.IsNullOrEmpty(subject))
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Object pronoun for {nick}: {IrcValues.BOLD}{@object}{IrcValues.RESET}");
                    }
                    else if (string.IsNullOrEmpty(@object))
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Subject pronoun for {nick}: {IrcValues.BOLD}{subject}{IrcValues.RESET}");
                    }
                    else
                    {
                        yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Preferred pronouns for {nick}: {IrcValues.BOLD}{subject}/{@object}{IrcValues.RESET}");
                    }
                }
            }
        }
    }
}
