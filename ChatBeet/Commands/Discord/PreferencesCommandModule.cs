using ChatBeet.Attributes;
using ChatBeet.Commands.Discord.Autocomplete;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using IF.Lastfm.Core.Api.Helpers;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Discord;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
[SlashCommandGroup("preference", "Commands for managing user preferences.")]
public class PreferencesCommandModule : ApplicationCommandModule
{
    private readonly UserPreferencesService _service;

    public PreferencesCommandModule(UserPreferencesService service)
    {
        _service = service;
    }

    [SlashCommand("get", "Check the value one of your preferences.")]
    public async Task GetPreferenceValue(InteractionContext ctx, [Option("preference", "Preference to check")] UserPreference preference)
    {
        var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;
        var value = await _service.Get(ctx.User, preference);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent(string.IsNullOrWhiteSpace(value)
            ? $"No preference is set for {Formatter.Italic(preference.ToString())}."
            : $"{Formatter.Italic(displayName)} is set to {Formatter.Bold(value)}")
            .AsEphemeral());
    }

    [SlashCommand("set", "Change the value one of your preferences.")]
    public async Task SetPreferenceValue(InteractionContext ctx, [Option("preference", "Preference to set")] UserPreference preference, [Option("value", "Value to set"), Autocomplete(typeof(PreferenceAutocompleteProvider))] string value) =>
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent(await SetPreferenceSilent(ctx, preference, value))
            .AsEphemeral());

    private async Task<string> SetPreferenceSilent(InteractionContext ctx, UserPreference preference, string value)
    {
        var validationMessage = _service.GetValidation(preference, value);

        if (!string.IsNullOrEmpty(validationMessage))
        {
            return validationMessage;
        }
        else
        {
            var normalized = await _service.Set(ctx.User, preference, value);
            return UserPreferencesService.GetDiscordConfirmationMessage(preference, normalized);
        }
    }
}
