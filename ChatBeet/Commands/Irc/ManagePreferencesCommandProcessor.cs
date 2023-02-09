using ChatBeet.Attributes;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using IF.Lastfm.Core.Api.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc;

public class ManagePreferencesCommandProcessor : CommandProcessor
{
    private readonly UserPreferencesService service;
    private static readonly Dictionary<string, UserPreference> preferenceMappings;

    static ManagePreferencesCommandProcessor()
    {
        preferenceMappings = Enum.GetValues(typeof(UserPreference))
            .Cast<UserPreference>()
            .ToDictionary(p => p.GetAttribute<ParameterAttribute>().InlineName, p => p);
    }

    public ManagePreferencesCommandProcessor(UserPreferencesService service)
    {
        this.service = service;
    }

    [Command("preference set {preferenceId}={value}", Description = "Set a user preference.")]
    public async Task<IClientMessage> SetPreference([Required] string preferenceId, [Required] string value)
    {
        if (preferenceMappings.ContainsKey(preferenceId))
        {
            var preference = preferenceMappings[preferenceId];
            var validationMessage = service.GetValidation(preference, value);

            if (!string.IsNullOrEmpty(validationMessage))
            {
                return new PrivateMessage(IncomingMessage.From, validationMessage);
            }
            else
            {
                var normalized = await service.Set(IncomingMessage.From, preference, value);
                return new PrivateMessage(IncomingMessage.From, UserPreferencesService.GetConfirmationMessage(preference, normalized));
            }
        }
        else
        {
            return new PrivateMessage(IncomingMessage.From, $"No preference {IrcValues.ITALIC}{preferenceId}{IrcValues.RESET} exists.");
        }
    }

    [Command("preference get {preferenceId}", Description = "Get a user preference.")]
    public async Task<IClientMessage> GetPreference([Required] string preferenceId)
    {
        if (preferenceMappings.ContainsKey(preferenceId))
        {
            var preference = preferenceMappings[preferenceId];
            var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;
            var value = await service.Get(IncomingMessage.From, preference);
            return new PrivateMessage(IncomingMessage.From, $"{IrcValues.ITALIC}{displayName}{IrcValues.RESET} is set to {IrcValues.BOLD}{value}{IrcValues.RESET}");
        }
        else
        {
            return new PrivateMessage(IncomingMessage.From, $"No preference {IrcValues.ITALIC}{preferenceId}{IrcValues.RESET} exists.");
        }
    }
}