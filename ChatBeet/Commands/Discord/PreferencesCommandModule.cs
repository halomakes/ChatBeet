using ChatBeet.Attributes;
using ChatBeet.Data.Entities;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using GravyBot;
using IF.Lastfm.Core.Api.Helpers;
using System;
using System.Threading.Tasks;
using UnitsNet.Units;

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

    /*[SlashCommand("get", "Check the value one of your preferences.")]
    public async Task GetPreferenceValue(InteractionContext ctx, [Option("preference", "Preference to check")] UserPreference preference)
    {
        var displayName = preference.GetAttribute<ParameterAttribute>().DisplayName;
        var value = await _service.Get(ctx.User, preference);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent(string.IsNullOrWhiteSpace(value)
            ? $"No preference is set for {Formatter.Italic(preference.ToString())}."
            : $"{Formatter.Italic(displayName)} is set to {Formatter.Bold(value)}"));
    }*/

    [SlashModuleLifespan(SlashModuleLifespan.Scoped)]
    [SlashCommandGroup("set", "Commands for setting user preferences.")]
    public class SetPreferenceCommandModule : ApplicationCommandModule
    {
        private readonly UserPreferencesService _service;

        public SetPreferenceCommandModule(UserPreferencesService service)
        {
            _service = service;
        }
        /*
        [SlashCommand("work-hours", "Set your working hours.")]
        public async Task SetWorkHours(InteractionContext ctx, [Option("start-time", "Time of day that you begin work")] string start, [Option("end-time", "Time of day that you end work")] string end)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent(await SetPreferenceSilent(ctx, UserPreference.WorkHoursStart, start) + Environment.NewLine + await SetPreferenceSilent(ctx, UserPreference.WorkHoursStart, end)));
        }*/

        [SlashCommand("weather-location", "Set your location for weather reports.")]
        public async Task SetWeatherLocation(InteractionContext ctx, [Option("postal-code", "Postal code to get weather for")] string zipCode) => await SetPreference(ctx, UserPreference.WeatherLocation, zipCode);
        
        [SlashCommand("weather-units", "Set your preferred units for weather.")]
        public async Task SetWeatherLocation(InteractionContext ctx, [Option("temp", "Temperature unit")] TemperatureUnit tempUnit,
            [Option("precip", "Precipitation unit")] LengthUnit precipUnit, [Option("wind", "Windspeed unit")] SpeedUnit speedUnit)
        {
            var responses = new[]
            {
                await SetPreferenceSilent(ctx, UserPreference.WeatherTempUnit, tempUnit.ToString()),
                await SetPreferenceSilent(ctx, UserPreference.WeatherPrecipUnit, precipUnit.ToString()),
                await SetPreferenceSilent(ctx, UserPreference.WeatherWindUnit, speedUnit.ToString())
            };
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
                .WithContent(string.Join(Environment.NewLine, responses)));
        }

        [SlashCommand("crewmate-color", "Set the color for your crewmate in the Web UI.")]
        public Task SetColor(InteractionContext ctx, [Option("color", "Color in hex format")] string color) => SetPreference(ctx, UserPreference.GearColor, color);
        
        private async Task SetPreference(InteractionContext ctx, UserPreference preference, string value) => await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent(await SetPreferenceSilent(ctx, preference, value)));

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
}
