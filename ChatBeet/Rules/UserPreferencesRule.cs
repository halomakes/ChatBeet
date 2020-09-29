using ChatBeet.Attributes;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
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
    public class UserPreferencesRule : IAsyncMessageRule<PrivateMessage>, IMessageRule<PreferenceChange>
    {
        private readonly UserPreferencesService service;
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
            var botConfig = options.Value;
            rgx = new Regex($"^({Regex.Escape(botConfig.Nick)}, |{Regex.Escape(botConfig.CommandPrefix)})set (.*?)=(.*)", RegexOptions.IgnoreCase);
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var prefName = match.Groups[2].Value.Trim().ToLower();
                var value = match.Groups[3].Value.Trim();

                if (preferenceMappings.ContainsKey(prefName))
                {
                    var preference = preferenceMappings[prefName];
                    var validationMessage = service.GetValidation(preference, value);

                    if (!string.IsNullOrEmpty(validationMessage))
                    {
                        yield return new PrivateMessage(incomingMessage.From, validationMessage);
                    }
                    else
                    {
                        var normalized = await service.Set(incomingMessage.From, preference, value);
                        yield return new PrivateMessage(incomingMessage.From, GetConfirmationMessage(preference, normalized));
                    }
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.From, $"No preference {IrcValues.ITALIC}{prefName}{IrcValues.RESET} exists.");
                }
            }
        }

        public IEnumerable<IClientMessage> Respond(PreferenceChange incomingMessage)
        {
            yield return new PrivateMessage(incomingMessage.Nick, $"{GetConfirmationMessage(incomingMessage.Preference.Value, incomingMessage.Value)} via WebUI");
        }

        public IEnumerable<IClientMessage> Respond(object incomingMessage) => incomingMessage is PreferenceChange pc ? Respond(pc) : Enumerable.Empty<IClientMessage>();

        private string GetConfirmationMessage(UserPreference preference, string value)
        {
            var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;
            return string.IsNullOrEmpty(value)
                ? $"Cleared value for {IrcValues.ITALIC}{displayName}{IrcValues.RESET}"
                : $"Set {IrcValues.ITALIC}{displayName}{IrcValues.RESET} to {IrcValues.BOLD}{value}{IrcValues.RESET}";
        }
    }
}
