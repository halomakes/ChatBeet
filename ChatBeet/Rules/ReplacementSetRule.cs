using ChatBeet.Configuration;
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
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class ReplacementSetRule : MessageRuleBase<PrivateMessage>
    {
        private readonly ReplacementContext db;
        private readonly MessageQueueService messageQueue;
        private readonly IrcBotConfiguration config;
        private readonly NegativeResponseService negativeResponseService;
        private readonly Regex rgx;
        private static IEnumerable<ReplacementSet> ReplacementSets;
        private static DateTime LastRefreshed;

        private static readonly TimeSpan RefreshInterval = TimeSpan.FromMinutes(5);

        public ReplacementSetRule(ReplacementContext db, MessageQueueService messageQueue, IOptions<IrcBotConfiguration> opts, NegativeResponseService nrService)
        {
            this.db = db;
            this.messageQueue = messageQueue;
            config = opts.Value;
            negativeResponseService = nrService;
            rgx = new Regex($@"^{Regex.Escape(config.CommandPrefix)}([^\ ]+)\ ([^\ ]+)", RegexOptions.IgnoreCase);
            Initialize();
        }

        private async void Initialize()
        {
            await RefreshMappings();
        }

        public override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            if (match.Success && ReplacementSets != default && ReplacementSets.Any())
            {
                var setName = match.Groups[1].Value.ToLower();
                var nick = match.Groups[2].Value;
                if (!string.IsNullOrEmpty(setName))
                {
                    var set = ReplacementSets.FirstOrDefault(s => s.Command.ToLower() == setName);
                    if (set != default)
                    {
                        var lookupMessage = messageQueue.GetLatestMessage(nick, incomingMessage.To, incomingMessage);
                        if (lookupMessage != default)
                        {
                            var content = lookupMessage.Message;
                            foreach (var map in set.Mappings.OrderByDescending(m => m.Input.Length))
                            {
                                content = content.Replace(map.Input, map.Replacement, true, ChatBeetConfiguration.Culture);
                            }
                            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {content}");
                        }
                        else if (nick == config.Nick)
                        {
                            yield return negativeResponseService.GetResponse(incomingMessage);
                        }
                    }
                }
            }
        }

        private async Task RefreshMappings()
        {
            if (ReplacementSets == default || (DateTime.Now - LastRefreshed > RefreshInterval))
            {
                ReplacementSets = await db.Sets
                    .Include(s => s.Mappings)
                    .ToListAsync();
                LastRefreshed = DateTime.Now;
            }
        }
    }
}