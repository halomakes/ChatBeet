using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using OpenWeatherMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnitsNet.Units;
using OpenWeatherMapClient = OpenWeatherMap.Standard.Current;

namespace ChatBeet.Rules
{
    public class CurrentWeatherRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly OpenWeatherMapClient wmClient;
        private readonly IrcBotConfiguration botConfig;
        private readonly UserPreferencesService prefsService;
        private readonly Regex rgx;

        public CurrentWeatherRule(IOptions<IrcBotConfiguration> options, OpenWeatherMapClient wmClient, UserPreferencesService prefsService)
        {
            botConfig = options.Value;
            this.wmClient = wmClient;
            this.prefsService = prefsService;
            rgx = new Regex($"{Regex.Escape(botConfig.CommandPrefix)}weather( .*)?");
        }

        public bool Matches(PrivateMessage incomingMessage) => rgx.IsMatch(incomingMessage.Message);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var match = rgx.Match(incomingMessage.Message);
            var zip = match.Groups[1].Value?.Trim();

            if (string.IsNullOrEmpty(zip))
            {
                zip = await prefsService.Get(incomingMessage.From, UserPreference.WeatherLocation);
            }
            else
            {
                var valMsg = prefsService.GetValidation(UserPreference.WeatherLocation, zip);
                if (!string.IsNullOrEmpty(valMsg))
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), valMsg);
                    zip = default;
                }
            }

            if (!string.IsNullOrEmpty(zip))
            {
                var currentConditions = await wmClient.GetWeatherDataByZipAsync(zip, "US");
                if (currentConditions != default)
                {
                    var windUnit = await prefsService.Get(incomingMessage.From, UserPreference.WeatherWindUnit, SpeedUnit.MilePerHour, SpeedUnit.Undefined);
                    var tempUnit = await prefsService.Get(incomingMessage.From, UserPreference.WeatherTempUnit, TemperatureUnit.DegreeFahrenheit, TemperatureUnit.Undefined);
                    var precipUnit = await prefsService.Get(incomingMessage.From, UserPreference.WeatherPrecipUnit, LengthUnit.Inch, LengthUnit.Undefined);

                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Current conditions for {IrcValues.BOLD}{currentConditions.Name}{IrcValues.RESET}");
                }
                else
                {
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Couldn't find any weather data for ZIP code {IrcValues.BOLD}{zip}{IrcValues.RESET}.");
                }
            }
            else
            {
                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), "Please specify a ZIP code or set a default one in your user preferences.");
            }
        }
    }
}
