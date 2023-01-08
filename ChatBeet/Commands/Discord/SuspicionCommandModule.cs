using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashCommandGroup("suspicion", "Commands related to suspicion levels")]
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class SuspicionCommandModule : ApplicationCommandModule
{
    private readonly SuspicionContext db;
    private readonly UserPreferencesService prefsService;
    private readonly DiscordClient client;
    private readonly NegativeResponseService negativeResponseService;

    public SuspicionCommandModule(SuspicionContext db, UserPreferencesService prefsService, DiscordClient client, NegativeResponseService negativeResponseService)
    {
        this.db = db;
        this.prefsService = prefsService;
        this.negativeResponseService = negativeResponseService;
        this.client = client;
    }

    [SlashCommand("report", "Report a user as being suspicious.")]
    public async Task IncreaseSuspicion(InteractionContext ctx, [Option("suspect", "Person who is being a sussy baka")] DiscordUser suspect)
    {
        if (suspect.Equals(client.CurrentUser))
        {
            await negativeResponseService.Respond(ctx);
        }
        else
        {
            if (await db.HasRecentlyReportedAsync(suspect.DiscriminatedUsername(), ctx.User.DiscriminatedUsername()))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                    .WithTitle("Slow down there, partner.")
                    .WithContent("You must wait at least 2 minutes each time you raise suspicion against a user."));
            }
            else
            {
                await db.ReportSuspiciousActivityAsync(suspect.DiscriminatedUsername(), ctx.User.DiscriminatedUsername(), bypassDebounceCheck: true);

                var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.DiscriminatedUsername());

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"{Formatter.Mention(suspect)}{suspect.Username.GetPossiveSuffix()} suspicion level is now {suspicionLevel}."));
            }
        }
    }

    [SlashCommand("check", "Check how suspicious a user is.")]
    public async Task GetSuspicionLevel(InteractionContext ctx, [Option("suspect", "Person who is being a sussy baka")] DiscordUser suspect)
    {
        var suspicionLevel = await db.GetSuspicionLevelAsync(suspect.DiscriminatedUsername());
        var maxLevel = await db.ActiveSuspicions.GroupBy(s => s.Suspect.ToLower())
            .Select(s => s.Count())
            .MaxAsync();

        var descriptor = GetSuspicionDescriptor(suspicionLevel, maxLevel);

        string comment = string.Empty;
        if (!string.IsNullOrEmpty(descriptor))
        {
            var pronounPref = await prefsService.Get(suspect.DiscriminatedUsername(), UserPreference.SubjectPronoun);
            var subjectPhrase = GetSubjectPhrase(pronounPref);
            comment = $" {subjectPhrase} {descriptor}.";
        }
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"{Formatter.Mention(suspect)}{suspect.Username.GetPossiveSuffix()} suspicion level is {suspicionLevel}.{comment}"));

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
