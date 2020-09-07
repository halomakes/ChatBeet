using ChatBeet.Annotations;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using GravyBot;
using GravyIrc.Messages;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class UserPreferencesRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly UserPreferencesService service;
        private readonly IrcBotConfiguration config;
        private readonly Regex rgx;
        private static readonly Dictionary<string, UserPreference> preferenceMappings;

        static UserPreferencesRule()
        {
            preferenceMappings = Enum.GetValues(typeof(UserPreference))
                .Cast<UserPreference>()
                .ToDictionary(p => p.GetAttribute<ParameterAttribute>().InlineName, p => p);
        }

        public UserPreferencesRule(IOptions<IrcBotConfiguration> options, UserPreferencesService service)
        {
            this.service = service;
            config = options.Value;
            rgx = new Regex($"^({Regex.Escape(config.Nick)}, |{Regex.Escape(config.CommandPrefix)})set (.*?)=(.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var pref = match.Groups[2].Value.Trim().ToLower();
                var value = match.Groups[3].Value.Trim();

                if (preferenceMappings.ContainsKey(pref))
                {
                    var preference = preferenceMappings[pref];
                    await service.Set(incomingMessage.From, preference, value);
                    var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;

                    if (string.IsNullOrEmpty(value))
                    {
                        yield return new PrivateMessage(incomingMessage.From, $"Cleared value for {IrcValues.ITALIC}{displayName}{IrcValues.RESET}.");
                    }
                    else
                    {
                        yield return new PrivateMessage(incomingMessage.From, $"Set {IrcValues.ITALIC}{displayName}{IrcValues.RESET} to {IrcValues.BOLD}{value}{IrcValues.RESET}.");
                    }
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.From, $"No preference {IrcValues.ITALIC}{pref}{IrcValues.RESET} exists.");
                }
            }
        }
    }
}
