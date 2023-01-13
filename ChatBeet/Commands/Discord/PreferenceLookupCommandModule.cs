using ChatBeet.Data.Entities;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class PreferenceLookupCommandModule : ApplicationCommandModule
{
    private readonly UserPreferencesService _preferences;
    private readonly IrcMigrationService _migration;

    public PreferenceLookupCommandModule(UserPreferencesService preferences, IrcMigrationService migration)
    {
        _preferences = preferences;
        _migration = migration;
    }
    private async Task GetPronouns(BaseContext ctx, DiscordUser user)
    {
        if (user == ctx.Client.CurrentUser)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent($"Just refer to {Formatter.Mention(user)} as 'best bot'.")
                .AsEphemeral());
        }
        else
        {
            var internalId = await _migration.GetInternalUsername(user);
            var subject = (await _preferences.Get(internalId, UserPreference.SubjectPronoun))?.ToLower();
            var @object = (await _preferences.Get(internalId, UserPreference.ObjectPronoun))?.ToLower();

            string content;
            if (string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(@object))
                content = $"Sorry, I don't know the preferred pronouns for {Formatter.Mention(user)}.";
            else if (string.IsNullOrEmpty(subject))
                content = $"Object pronoun for {Formatter.Mention(user)}: {Formatter.Bold(@object)}";
            else if (string.IsNullOrEmpty(@object))
                content = $"Subject pronoun for {Formatter.Mention(user)}: {Formatter.Bold(subject)}";
            else
                content = $"Preferred pronouns for {Formatter.Mention(user)}: {Formatter.Bold(subject)}/{Formatter.Bold(@object)}";

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(content)
                .AsEphemeral());
        }
    }

    [ContextMenu(ApplicationCommandType.UserContextMenu, "View pronouns")]
    public Task GetPronouns(ContextMenuContext ctx) => GetPronouns(ctx, ctx.TargetUser);

    [SlashCommand("pronouns", "See what a user's preferred pronouns are")]
    public async Task LookupUser(InteractionContext ctx, [Option("user", "User to look up")] DiscordUser user) => await GetPronouns(ctx, user);
}