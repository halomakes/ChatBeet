using ChatBeet.Commands.Irc;
using ChatBeet.Data;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public partial class SuspectRule : CommandAliasRule<SuspicionCommandProcessor>, IAsyncMessageRule<MessageCreateEventArgs>
    {
        private readonly DiscordClient _discord;
        private readonly NegativeResponseService _negativeResponseService;
        private readonly SuspicionContext _db;

        public SuspectRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider, DiscordClient discord, NegativeResponseService negativeResponseService, SuspicionContext db) : base(options, serviceProvider)
        {
            Pattern = new Regex($@"(?:^({RegexUtils.Nick})(?: is)? sus$)");
            _discord = discord;
            _negativeResponseService = negativeResponseService;
            _db = db;
        }

        public bool Matches(MessageCreateEventArgs incomingMessage) => discordRgx().IsMatch(incomingMessage.Message.Content);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(MessageCreateEventArgs incomingMessage)
        {
            var suspect = incomingMessage.MentionedUsers.Single();
            if (suspect.Equals(_discord.CurrentUser))
            {
                await _negativeResponseService.Respond(incomingMessage.Message);
            }
            else
            {
                if (await _db.HasRecentlyReportedAsync(suspect.DiscriminatedUsername(), incomingMessage.Author.DiscriminatedUsername()))
                {
                    await incomingMessage.Message.RespondAsync("You must wait at least 2 minutes each time you raise suspicion against a user.");
                }
                else
                {
                    await _db.ReportSuspiciousActivityAsync(suspect.DiscriminatedUsername(), incomingMessage.Author.DiscriminatedUsername(), bypassDebounceCheck: true);

                    var suspicionLevel = await _db.GetSuspicionLevelAsync(suspect.DiscriminatedUsername());

                    await incomingMessage.Message.RespondAsync($"{Formatter.Mention(suspect)}{suspect.Username.GetPossiveSuffix()} suspicion level is now {suspicionLevel}.");
                }
            }
            await incomingMessage.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🕵"));
            yield break;
        }

        protected override IAsyncEnumerable<IClientMessage> OnMatch(Match match, SuspicionCommandProcessor commandProcessor) =>
            commandProcessor.IncreaseSuspicion(match.Groups[1].Value);

        [GeneratedRegex(@"^\<@\d+\> sus$")]
        private partial Regex discordRgx();
    }
}
