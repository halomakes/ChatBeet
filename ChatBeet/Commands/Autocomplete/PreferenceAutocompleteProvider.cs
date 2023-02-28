using System.Collections.Generic;
using System.Threading.Tasks;
using ChatBeet.Configuration;
using ChatBeet.Data.Entities;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UnitsNet.Units;

namespace ChatBeet.Commands.Autocomplete;

public class PreferenceAutocompleteProvider : IAutocompleteProvider
{
    private const int MaxResults = 25;

    public async Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx) =>
        (await GetOptions(ctx))
        .Select(s => new DiscordAutoCompleteChoice(s, s));

    private async Task<IEnumerable<string>> GetOptions(AutocompleteContext ctx)
    {
        await using var scope = ctx.Services.CreateAsyncScope();
        var config = scope.ServiceProvider.GetRequiredService<IOptions<ChatBeetConfiguration>>().Value;
        var preferenceType = ctx.Options[0].Value as string;
        if (Enum.TryParse<UserPreference>(preferenceType, out var type))
        {
            switch (type)
            {
                case UserPreference.ObjectPronoun:
                    return config.Pronouns.Allowed.Objects;
                case UserPreference.PossessivePronoun:
                    return config.Pronouns.Allowed.Possessives;
                case UserPreference.ReflexivePronoun:
                    return config.Pronouns.Allowed.Reflexives;
                case UserPreference.SubjectPronoun:
                    return config.Pronouns.Allowed.Subjects;
                case UserPreference.WeatherPrecipUnit:
                    return SearchEnumOptions<LengthUnit>(ctx);
                case UserPreference.WeatherTempUnit:
                    return SearchEnumOptions<TemperatureUnit>(ctx);
                case UserPreference.WeatherWindUnit:
                    return SearchEnumOptions<SpeedUnit>(ctx);
            }
        }
        return Enumerable.Empty<string>();
    }

    private IEnumerable<string> SearchEnumOptions<TEnum>(AutocompleteContext ctx) where TEnum : struct, Enum
    {
        var currentInput = ctx.FocusedOption.Value as string;
        if (currentInput is null)
            return Array.Empty<string>();
        return Enum.GetNames<TEnum>()
            .Select(c => new
            {
                Item = c,
                Rating = (c.ToLower().StartsWith(currentInput) ? 5 : 0)
                            + (c.ToLower().Contains(currentInput) ? 4 : 0)
            })
            .Where(r => r.Rating > 0)
                    .OrderByDescending(r => r.Rating)
                    .ThenBy(r => r.Item)
                    .Select(r => r.Item)
                    .Take(MaxResults);
    }
}
