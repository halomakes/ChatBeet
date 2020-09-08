using ChatBeet.Annotations;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class WorkdayProgressRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly UserPreferencesService preferences;
        private readonly string command;

        public WorkdayProgressRule(IOptions<IrcBotConfiguration> opts, UserPreferencesService preferences)
        {
            config = opts.Value;
            this.preferences = preferences;
            command = $"{config.CommandPrefix}progress workday";
        }

        public override bool Matches(PrivateMessage incomingMessage) => incomingMessage.Message.Equals(command, StringComparison.InvariantCultureIgnoreCase);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var startPref = await preferences.Get(incomingMessage.From, UserPreference.WorkHoursStart);
            var endPref = await preferences.Get(incomingMessage.From, UserPreference.WorkHoursEnd);

            if (!IsValidDate(startPref))
            {
                var description = UserPreference.WorkHoursStart.GetAttribute<ParameterAttribute>().DisplayName;
                yield return new PrivateMessage(incomingMessage.From, $"No valid value set for preference {IrcValues.ITALIC}{description}{IrcValues.RESET}");
            }
            else if (!IsValidDate(endPref))
            {
                var description = UserPreference.WorkHoursEnd.GetAttribute<ParameterAttribute>().DisplayName;
                yield return new PrivateMessage(incomingMessage.From, $"No valid value set for preference {IrcValues.ITALIC}{description}{IrcValues.RESET}");
            }
            else
            {
                var now = DateTime.Now;
                var start = NormalizeTime(DateTime.Parse(startPref), now);
                var end = NormalizeTime(DateTime.Parse(endPref), now);

                if (end < start)
                {
                    // handle overnight shifts
                    if (start < now)
                    {
                        end = end.AddDays(1);
                    }
                    else
                    {
                        start = start.AddDays(-1);
                    }
                }

                if (start <= now && end >= now)
                {
                    var bar = Progress.GetBar(now, start, end, $"{IrcValues.BOLD}Your workday{IrcValues.RESET} is");
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), bar);
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"You are outside of working hours.");
                }
            }

            static bool IsValidDate(string val) => !string.IsNullOrEmpty(val) && DateTime.TryParse(val, out var _);

            static DateTime NormalizeTime(DateTime date, DateTime baseline) =>
                new DateTime(baseline.Year, baseline.Month, baseline.Day, date.Hour, date.Minute, date.Second);
        }
    }
}
