using ChatBeet.Data;
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
    public class SuspectRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly Regex rgx;
        private readonly IrcBotConfiguration config;
        private readonly SuspicionContext db;
        private readonly NegativeResponseService negativeResponseService;

        public SuspectRule(IOptions<IrcBotConfiguration> options, SuspicionContext db, NegativeResponseService negativeResponseService)
        {
            config = options.Value;
            this.db = db;
            this.negativeResponseService = negativeResponseService;
            rgx = new Regex($@"(^{Regex.Escape(config.CommandPrefix)}sus(?:pect)? ({RegexUtils.Nick}))|(?:^({RegexUtils.Nick})(?: is)? sus$)");
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            if (incomingMessage.IsChannelMessage)
            {
                var match = rgx.Match(incomingMessage.Message);
                var suspect = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value;
                if (!string.IsNullOrEmpty(suspect))
                {
                    if (suspect.Trim().Equals(config.Nick, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return negativeResponseService.GetResponse(incomingMessage);
                    }
                    else
                    {
                        if (await db.HasRecentlyReportedAsync(suspect, incomingMessage.From))
                        {
                            yield return new PrivateMessage(incomingMessage.From, $"You must wait at least 2 minutes each time you raise suspicion against a user.");
                        }
                        else
                        {
                            await db.ReportSuspiciousActivityAsync(suspect, incomingMessage.From, bypassDebounceCheck: true);

                            var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.Trim());

                            yield return new PrivateMessage(incomingMessage.From, $"{suspect.ToPossessive()} suspicion level is now {suspicionLevel}.");

                            yield return new PrivateMessage(suspect, $"{incomingMessage.From} reported you as acting suspiciously. Your suspicion level is now {suspicionLevel}.");
                        }
                    }
                }
            }
            else
            {
                if (!await db.HasRecentlyReportedAsync(incomingMessage.From, config.Nick))
                {
                    db.Suspicions.Add(new Suspicion
                    {
                        Reporter = config.Nick,
                        Suspect = incomingMessage.From,
                        TimeReported = DateTime.Now
                    });
                    await db.SaveChangesAsync();
                    var suspicionLevel = await db.GetSuspicionLevelAsync(incomingMessage.From);
                    yield return new PrivateMessage(incomingMessage.From, $"Reporting someone in private messages is pretty sus. Your suspicion level just went up to {suspicionLevel}.");
                }
                else
                {
                    yield return negativeResponseService.GetResponse(incomingMessage);
                }
            }
        }
    }
}
