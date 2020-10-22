using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public class SuspicionLookupRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly Regex rgx;
        private readonly IrcBotConfiguration config;
        private readonly SuspicionContext db;
        private readonly UserPreferencesService prefsService;

        public SuspicionLookupRule(IOptions<IrcBotConfiguration> options, SuspicionContext db, UserPreferencesService prefsService)
        {
            config = options.Value;
            this.db = db;
            this.prefsService = prefsService;
            rgx = new Regex($@"(^{Regex.Escape(config.CommandPrefix)}suspicion ({RegexUtils.Nick}))");
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            var suspect = match.Groups[2].Value;
            if (!string.IsNullOrEmpty(suspect))
            {
                var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.Trim());
                var maxLevel = await db.Suspicions.AsQueryable().GroupBy(s => s.Suspect).MaxAsync(g => g.Count());

                var descriptor = GetSuspicionDescriptor(suspicionLevel, maxLevel);

                string comment = string.Empty;
                if (!string.IsNullOrEmpty(descriptor))
                {
                    var pronounPref = await prefsService.Get(suspect, UserPreference.SubjectPronoun);
                    var subjectPhrase = GetSubjectPhrase(pronounPref);
                    comment = $" {subjectPhrase} {descriptor}.";
                }

                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{suspect.ToPossessive()} suspicion level is {suspicionLevel}.{comment}");
            }

            static string GetSubjectPhrase(string pronounPreference)
            {
                if (string.IsNullOrEmpty(pronounPreference))
                    return "That's";

                if (pronounPreference.Equals("they", StringComparison.OrdinalIgnoreCase))
                    return $"{pronounPreference.CapitalizeFirst()} are";
                else
                    return $"{pronounPreference.CapitalizeFirst()} is";
            }

            static string GetSuspicionDescriptor(int level, int maxLevel)
            {
                if (level == 0)
                    return "not sus at all";

                if (maxLevel == 0)
                    return default;

                if (level == maxLevel)
                    return "maximum sus";

                var ratio = (double)(level * 100) / maxLevel;

                if (ratio > 90)
                    return "ultra sus";
                else if (ratio > 80)
                    return "mega sus";
                else if (ratio > 70)
                    return "mad sus";
                else if (ratio > 60)
                    return "very sus";
                else if (ratio > 50)
                    return "pretty sus ngl";
                else if (ratio > 40)
                    return "sus if I've ever seen it";
                else if (ratio > 30)
                    return "a little sus";
                else if (ratio > 20)
                    return "slightly sus I guess";
                else if (ratio > 10)
                    return "the teeniest bit sus";
                else
                    return "not sus enough to really count";
            }
        }
    }
}
