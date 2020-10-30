using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnitsNet;
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
            rgx = new Regex($"{Regex.Escape(botConfig.CommandPrefix)}weather( \\d{{5}})?");
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
                    var complicationDetails = new List<string>();

                    var windUnit = await prefsService.Get(incomingMessage.From, UserPreference.WeatherWindUnit, SpeedUnit.MilePerHour, SpeedUnit.Undefined);
                    var tempUnit = await prefsService.Get(incomingMessage.From, UserPreference.WeatherTempUnit, TemperatureUnit.DegreeFahrenheit, TemperatureUnit.Undefined);
                    var precipUnit = await prefsService.Get(incomingMessage.From, UserPreference.WeatherPrecipUnit, LengthUnit.Inch, LengthUnit.Undefined);

                    // temperature
                    string getTemp(float c) => Temperature.FromDegreesCelsius(c).ToUnit(tempUnit).ToString();
                    complicationDetails.Add($"🌡{IrcValues.BOLD}{getTemp(currentConditions.WeatherDayInfo.Temperature)}{IrcValues.RESET}{IrcValues.RED} ⬆{getTemp(currentConditions.WeatherDayInfo.MaximumTemperature)}{IrcValues.BLUE} ⬇{getTemp(currentConditions.WeatherDayInfo.MinimumTemperature)}{IrcValues.RESET}");

                    // humidity
                    var idealHumidity = 40;
                    var comfortRating = Math.Abs(100 - Math.Abs(currentConditions.WeatherDayInfo.Humidity - idealHumidity));
                    complicationDetails.Add($"Humidity: {$"{currentConditions.WeatherDayInfo.Humidity}%".Colorize(comfortRating)}");

                    string getPrecipitation(float c) => string.Format("{0:s2}", Length.FromMillimeters(c).ToUnit(precipUnit));

                    // conditional rain
                    if (currentConditions.Rain != default)
                    {
                        complicationDetails.Add($"{getPrecipitation(currentConditions.Rain.LastHour)} rain in past hour");
                    }

                    // conditional snow
                    if (currentConditions.Snow != default)
                    {
                        complicationDetails.Add($"{getPrecipitation(currentConditions.Snow.LastHour)} snow in past hour");
                    }

                    // conditional wind
                    if (currentConditions.Wind != default)
                    {
                        string getWindspeed(float c) => string.Format("{0:s1}", Speed.FromMetersPerSecond(c).ToUnit(windUnit));
                        string windDirection = Angle.FromDegrees(currentConditions.Wind.Degree).ToCardinalDirection();
                        complicationDetails.Add($"Wind: {getWindspeed(currentConditions.Wind.Speed)} {windDirection}");
                    }

                    // conditions
                    complicationDetails.Add(string.Join(", ", currentConditions.Weathers.Select(w => $"{w.GetEmoji(currentConditions.DayInfo)} {w.Description}")));

                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"Right now in {IrcValues.BOLD}{currentConditions.Name}{IrcValues.RESET}: {string.Join(" | ", complicationDetails)}");
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
