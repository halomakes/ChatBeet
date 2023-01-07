using ChatBeet.Data.Entities;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitsNet;
using UnitsNet.Units;
using OpenWeatherMapClient = OpenWeatherMap.Standard.Current;

namespace ChatBeet.Commands.Irc
{
    public class WeatherCommandProcessor : CommandProcessor
    {
        private readonly OpenWeatherMapClient wmClient;
        private readonly UserPreferencesService prefsService;

        public WeatherCommandProcessor(OpenWeatherMapClient wmClient, UserPreferencesService prefsService)
        {
            this.wmClient = wmClient;
            this.prefsService = prefsService;
        }

        [Command("weather {zipCode}", Description = "Get current weather conditions.")]
        public async Task<IClientMessage> GetCurrentConditions(string zipCode)
        {
            if (string.IsNullOrEmpty(zipCode))
            {
                zipCode = await prefsService.Get(IncomingMessage.From, UserPreference.WeatherLocation);
            }
            else
            {
                var valMsg = prefsService.GetValidation(UserPreference.WeatherLocation, zipCode);
                if (!string.IsNullOrEmpty(valMsg))
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), valMsg);
            }

            if (!string.IsNullOrEmpty(zipCode))
            {
                var currentConditions = await wmClient.GetWeatherDataByZipAsync(zipCode, "US");
                if (currentConditions != default)
                {
                    var complicationDetails = new List<string>();

                    var windUnit = await prefsService.Get(IncomingMessage.From, UserPreference.WeatherWindUnit, SpeedUnit.MilePerHour, SpeedUnit.Undefined);
                    var tempUnit = await prefsService.Get(IncomingMessage.From, UserPreference.WeatherTempUnit, TemperatureUnit.DegreeFahrenheit, TemperatureUnit.Undefined);
                    var precipUnit = await prefsService.Get(IncomingMessage.From, UserPreference.WeatherPrecipUnit, LengthUnit.Inch, LengthUnit.Undefined);

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

                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Right now in {IrcValues.BOLD}{currentConditions.Name}{IrcValues.RESET}: {string.Join(" | ", complicationDetails)}");
                }
                else
                {
                    return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"Couldn't find any weather data for ZIP code {IrcValues.BOLD}{zipCode}{IrcValues.RESET}.");
                }
            }
            else
            {
                return new PrivateMessage(IncomingMessage.GetResponseTarget(), "Please specify a ZIP code or set a default one in your user preferences.");
            }
        }
    }
}
