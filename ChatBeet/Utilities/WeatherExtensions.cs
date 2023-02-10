using OpenWeatherMap.Standard.Models;
using UnitsNet;

namespace ChatBeet.Utilities;

public static class WeatherExtensions
{
    public static Complication? GetComplication(this Weather weather)
    {
        if (weather.Id == 800)
            return Complication.Clear;
        var groupId = weather.Id / 100;

        if (Enum.IsDefined(typeof(Complication), groupId))
            return (Complication)groupId;

        return default;
    }
    public static string GetEmoji(this Weather weather, DayInfo dayInfo) => GetComplication(weather) switch
    {
        Complication.Clear => IsDay(dayInfo) ? "☀" : "🌙",
        Complication.Thunderstorm => "⛈",
        Complication.Drizzle => "🌦",
        Complication.Rain => "🌧",
        Complication.Snow => "🌨",
        Complication.Atmosphere => "🌫",
        Complication.Clouds => "☁",
        _ => default
    };

    public enum Complication
    {
        Clear = 0,
        Thunderstorm = 2,
        Drizzle = 3,
        Rain = 5,
        Snow = 6,
        Atmosphere = 7,
        Clouds = 8
    }

    public static bool IsDay(this DayInfo dayInfo)
    {
        var now = DateTime.Now;
        return now > dayInfo.Sunrise && now < dayInfo.Sunset;
    }

    public static string ToCardinalDirection(this Angle angle)
    {
        string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        return caridnals[(int)Math.Round((double)angle.Degrees * 10 % 3600 / 225)];
    }
}