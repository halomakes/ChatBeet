using System.Threading.Tasks;
using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace ChatBeet.Commands;

[SlashCommandGroup("suspicion", "Commands related to suspicion levels")]
[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class SuspicionCommandModule : ApplicationCommandModule
{
    private readonly SuspicionService _db;
    private readonly UserPreferencesService _prefsService;
    private readonly DiscordClient _client;
    private readonly NegativeResponseService _negativeResponseService;
    private readonly IUsersRepository _users;

    public SuspicionCommandModule(SuspicionService db, UserPreferencesService prefsService, DiscordClient client, NegativeResponseService negativeResponseService, IUsersRepository users)
    {
        _db = db;
        _prefsService = prefsService;
        _negativeResponseService = negativeResponseService;
        _users = users;
        _client = client;
    }

    [SlashCommand("report", "Report a user as being suspicious")]
    public async Task IncreaseSuspicion(InteractionContext ctx, [Option("suspect", "Person who is being a sussy baka")] DiscordUser suspect)
    {
        if (suspect.Equals(_client.CurrentUser))
        {
            await _negativeResponseService.Respond(ctx);
        }
        else
        {
            var suspectId = (await _users.GetUserAsync(suspect)).Id;
            var userId = (await _users.GetUserAsync(ctx.User)).Id;

            if (await _db.HasRecentlyReportedAsync(ctx.Guild.Id, suspectId, userId))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("You must wait at least 2 minutes each time you raise suspicion against a user."));
            }
            else
            {
                await _db.ReportSuspiciousActivityAsync(ctx.Guild.Id, suspectId, userId, bypassDebounceCheck: true);

                var suspicionLevel = await _db.GetSuspicionLevelAsync(ctx.Guild.Id, suspectId);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent($"{Formatter.Mention(suspect)}{suspect.Username.GetPossiveSuffix()} suspicion level is now {suspicionLevel}."));
            }
        }
    }

    [SlashCommand("check", "Check how suspicious a user is")]
    public async Task GetSuspicionLevel(InteractionContext ctx, [Option("suspect", "Person who is being a sussy baka")] DiscordUser suspect)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(await GetSuspicionResponse(suspect, ctx.Guild.Id)));
    }

    private async Task<string> GetSuspicionResponse(DiscordUser suspect, ulong guildId)
    {
        var suspectId = (await _users.GetUserAsync(suspect)).Id;
        var suspicionLevel = await _db.GetSuspicionLevelAsync(guildId, suspectId);
        var maxLevel = (await _db.GetActiveSuspicionsAsync(guildId)).GroupBy(s => s.SuspectId)
            .Select(s => s.Count())
            .Max();

        var descriptor = GetSuspicionDescriptor(suspicionLevel, maxLevel);

        string comment = string.Empty;
        if (!string.IsNullOrEmpty(descriptor))
        {
            var pronounPref = await _prefsService.Get(suspectId, UserPreference.SubjectPronoun);
            var subjectPhrase = GetSubjectPhrase(pronounPref);
            comment = $" {subjectPhrase} {descriptor}.";
        }

        return $"{Formatter.Mention(suspect)}{suspect.Username.GetPossiveSuffix()} suspicion level is {suspicionLevel}.{comment}";

        static string GetSubjectPhrase(string pronounPreference)
        {
            if (string.IsNullOrEmpty(pronounPreference))
                return "That's";

            return pronounPreference.Equals("they", StringComparison.OrdinalIgnoreCase)
                ? $"{pronounPreference.CapitalizeFirst()} are"
                : $"{pronounPreference.CapitalizeFirst()} is";
        }

        static string? GetSuspicionDescriptor(int level, int maxLevel)
        {
            if (level == 0)
                return "not sus at all";

            if (maxLevel == 0)
                return default;

            if (level == maxLevel)
                return "maximum sus";

            var ratio = (double)(level * 100) / maxLevel;

            return ratio switch
            {
                > 90 => "ultra sus",
                > 80 => "mega sus",
                > 70 => "mad sus",
                > 60 => "very sus",
                > 50 => "pretty sus ngl",
                > 40 => "sus if I've ever seen it",
                > 30 => "a little sus",
                > 20 => "slightly sus I guess",
                > 10 => "the teeniest bit sus",
                _ => "not sus enough to really count"
            };
        }
    }
}