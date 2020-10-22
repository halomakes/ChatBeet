using ChatBeet.Data;
using ChatBeet.Data.Entities;
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

        public SuspectRule(IOptions<IrcBotConfiguration> options, SuspicionContext db)
        {
            config = options.Value;
            this.db = db;
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
                    db.Suspicions.Add(new Suspicion
                    {
                        Reporter = incomingMessage.From,
                        Suspect = suspect.Trim(),
                        TimeReported = DateTime.Now
                    });
                    await db.SaveChangesAsync();

                    var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.Trim());

                    yield return new PrivateMessage(incomingMessage.From, $"{suspect.ToPossessive()} suspicion level is now {suspicionLevel}.");

                    yield return new PrivateMessage(suspect, $"{incomingMessage.From} reported you as acting suspicously. Your suspicion level is now {suspicionLevel}.");
                }
            }
            else
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
        }
    }
}
