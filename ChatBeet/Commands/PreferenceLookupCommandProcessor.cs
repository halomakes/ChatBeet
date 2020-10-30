using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class PreferenceLookupCommandProcessor : CommandProcessor
    {
        private readonly UserPreferencesService userPreferences;
        private readonly IrcBotConfiguration config;
        private readonly NegativeResponseService negativeResponseService;

        public PreferenceLookupCommandProcessor(IOptions<IrcBotConfiguration> options, UserPreferencesService userPreferences, NegativeResponseService negativeResponseService)
        {
            config = options.Value;
            this.userPreferences = userPreferences;
            this.negativeResponseService = negativeResponseService;
        }

        [Command("pronouns {nick}", Description = "Get preferred pronouns for a user.")]
        public async Task<IClientMessage> GetPronouns(string nick)
        {
            if (nick.Equals(config.Nick, StringComparison.InvariantCultureIgnoreCase))
            {
                return negativeResponseService.GetResponse(IncomingMessage);
            }
            else
            {
                var subject = await userPreferences.Get(nick, UserPreference.SubjectPronoun);
                var @object = await userPreferences.Get(nick, UserPreference.ObjectPronoun);
                if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(@object))
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Sorry, I don't know the preferred pronouns for {nick}.");
                }
                else if (string.IsNullOrEmpty(subject))
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Object pronoun for {nick}: {IrcValues.BOLD}{@object}{IrcValues.RESET}");
                }
                else if (string.IsNullOrEmpty(@object))
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Subject pronoun for {nick}: {IrcValues.BOLD}{subject}{IrcValues.RESET}");
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Preferred pronouns for {nick}: {IrcValues.BOLD}{subject}/{@object}{IrcValues.RESET}");
                }
            }
        }
    }
}
