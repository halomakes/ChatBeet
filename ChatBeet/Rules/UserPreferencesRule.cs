using ChatBeet.Annotations;
using ChatBeet.Configuration;
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
        private readonly ChatBeetConfiguration config;
        private readonly Regex rgx;
        private static readonly Dictionary<string, UserPreference> preferenceMappings;

        static UserPreferencesRule()
        {
            preferenceMappings = Enum.GetValues(typeof(UserPreference))
                .Cast<UserPreference>()
                .ToDictionary(p => p.GetAttribute<ParameterAttribute>().InlineName, p => p);
        }

        public UserPreferencesRule(IOptions<IrcBotConfiguration> options, UserPreferencesService service, IOptions<ChatBeetConfiguration> cbOptions)
        {
            this.service = service;
            var botConfig = options.Value;
            config = cbOptions.Value;
            rgx = new Regex($"^({Regex.Escape(botConfig.Nick)}, |{Regex.Escape(botConfig.CommandPrefix)})set (.*?)=(.*)", RegexOptions.IgnoreCase);
        }

        public override bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success)
            {
                var prefName = match.Groups[2].Value.Trim().ToLower();
                var value = match.Groups[3].Value.Trim();

                if (preferenceMappings.ContainsKey(prefName))
                {
                    var preference = preferenceMappings[prefName];
                    var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;
                    var validationMessage = string.IsNullOrEmpty(value) ? default : preference switch
                    {
                        UserPreference.SubjectPronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Subjects, displayName),
                        UserPreference.ObjectPronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Objects, displayName),
                        UserPreference.PossessivePronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Possessives, displayName),
                        UserPreference.ReflexivePronoun => GetCollectionValidation(value, config.Pronouns.Allowed.Reflexives, displayName),
                        UserPreference.DateOfBirth => GetDateValidation(value),
                        UserPreference.WorkHoursEnd => GetDateValidation(value),
                        UserPreference.WorkHoursStart => GetDateValidation(value),
                        _ => default
                    };

                    if (!string.IsNullOrEmpty(validationMessage))
                    {
                        yield return new PrivateMessage(incomingMessage.From, validationMessage);
                    }
                    else
                    {
                        await service.Set(incomingMessage.From, preference, value);

                        if (string.IsNullOrEmpty(value))
                        {
                            yield return new PrivateMessage(incomingMessage.From, $"Cleared value for {IrcValues.ITALIC}{displayName}{IrcValues.RESET}.");
                        }
                        else
                        {
                            yield return new PrivateMessage(incomingMessage.From, $"Set {IrcValues.ITALIC}{displayName}{IrcValues.RESET} to {IrcValues.BOLD}{value}{IrcValues.RESET}.");
                        }
                    }
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.From, $"No preference {IrcValues.ITALIC}{prefName}{IrcValues.RESET} exists.");
                }
            }
        }

        private static string GetCollectionValidation(string value, IEnumerable<string> collection, string displayName)
        {
            if (!collection.Contains(value.ToLower()))
            {
                return $"Sorry, {IrcValues.BOLD}{value}{IrcValues.RESET} is not an available value for {IrcValues.ITALIC}{displayName}{IrcValues.RESET}.  Available values are [{string.Join(", ", collection)}].";
            }
            return default;
        }

        private static string GetDateValidation(string value)
        {
            if (!DateTime.TryParse(value, out var _))
            {
                return $"{IrcValues.BOLD}{value}{IrcValues.RESET} is not a valid date.";
            }
            return default;
        }
    }
}
