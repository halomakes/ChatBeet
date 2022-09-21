using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands
{
    public class SuspicionCommandProcessor : CommandProcessor
    {
        private readonly SuspicionContext db;
        private readonly UserPreferencesService prefsService;
        private readonly IrcBotConfiguration config;
        private readonly NegativeResponseService negativeResponseService;

        public SuspicionCommandProcessor(SuspicionContext db, UserPreferencesService prefsService, IOptions<IrcBotConfiguration> opts, NegativeResponseService negativeResponseService)
        {
            this.db = db;
            this.prefsService = prefsService;
            this.negativeResponseService = negativeResponseService;
            config = opts.Value;
        }

        [Command("suspect {suspect}", Description = "Report a user as being suspicious.")]
        public async IAsyncEnumerable<IClientMessage> IncreaseSuspicion([Required] string suspect)
        {
            if (IncomingMessage.IsChannelMessage)
            {
                if (!string.IsNullOrEmpty(suspect))
                {
                    if (suspect.Trim().Equals(config.Nick, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return negativeResponseService.GetResponse(IncomingMessage);
                    }
                    else
                    {
                        if (await db.HasRecentlyReportedAsync(suspect, IncomingMessage.From))
                        {
                            yield return new NoticeMessage(IncomingMessage.From, $"You must wait at least 2 minutes each time you raise suspicion against a user.");
                        }
                        else
                        {
                            await db.ReportSuspiciousActivityAsync(suspect, IncomingMessage.From, bypassDebounceCheck: true);

                            var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.Trim());

                            yield return new NoticeMessage(IncomingMessage.From, $"{suspect.ToPossessive()} suspicion level is now {suspicionLevel}.");

                            yield return new NoticeMessage(suspect, $"{IncomingMessage.From} reported you as acting suspiciously. Your suspicion level is now {suspicionLevel}.");
                        }
                    }
                }
            }
            else
            {
                if (!await db.HasRecentlyReportedAsync(IncomingMessage.From, config.Nick))
                {
                    db.Suspicions.Add(new Suspicion
                    {
                        Reporter = config.Nick,
                        Suspect = IncomingMessage.From,
                        TimeReported = DateTime.Now
                    });
                    await db.SaveChangesAsync();
                    var suspicionLevel = await db.GetSuspicionLevelAsync(IncomingMessage.From);
                    yield return new PrivateMessage(IncomingMessage.From, $"Reporting someone in private messages is pretty sus. Your suspicion level just went up to {suspicionLevel}.");
                }
                else
                {
                    yield return negativeResponseService.GetResponse(IncomingMessage);
                }
            }
        }

        [Command("suspicion {suspect}", Description = "Check how suspicious a user is.")]
        public async Task<IClientMessage> GetSuspicionLevel([Required] string suspect)
        {
            if (!string.IsNullOrEmpty(suspect))
            {
                var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.Trim());
                var maxLevel = await db.ActiveSuspicions.GroupBy(s => s.Suspect.ToLower())
                    .Select(s => s.Count())
                    .MaxAsync();

                var descriptor = GetSuspicionDescriptor(suspicionLevel, maxLevel);

                string comment = string.Empty;
                if (!string.IsNullOrEmpty(descriptor))
                {
                    var pronounPref = await prefsService.Get(suspect, UserPreference.SubjectPronoun);
                    var subjectPhrase = GetSubjectPhrase(pronounPref);
                    comment = $" {subjectPhrase} {descriptor}.";
                }

                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{suspect.ToPossessive()} suspicion level is {suspicionLevel}.{comment}");
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"You didn't specify who to check.  That's kinda sus.");
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
