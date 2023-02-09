using ChatBeet.Models;
using ChatBeet.Services;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;

namespace ChatBeet.Rules;

public class UserPreferencesRule : IMessageRule<PreferenceChange>
{
    public IEnumerable<IClientMessage> Respond(PreferenceChange incomingMessage)
    {
        yield return new PrivateMessage(incomingMessage.Nick, $"{UserPreferencesService.GetConfirmationMessage(incomingMessage.Preference.Value, incomingMessage.Value)} via WebUI");
    }
}