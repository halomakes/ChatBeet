using Humanizer;
using System;
using System.Collections.Generic;

namespace ChatBeet.Utilities;

public static class Progress
{
    public const int BarLength = 25;

    public static string GetCompletionDescription(DateTime now, DateTime start, DateTime end, string periodDescription)
    {
        var ratio = GetRatio(now, start, end);
        var percentage = GetFormattedPercentage(ratio);

        return $"{periodDescription} {percentage} complete.";
    }

    public static string GetCompletionDescription(double ratio, string periodDescription)
    {
        var percentage = GetFormattedPercentage(ratio);

        return $"{periodDescription} {percentage} complete.";
    }

    public static string FormatTemplate(DateTime now, DateTime start, DateTime end, string template)
    {
        var elapsed = ForcePositive(now - start);
        var remaining = ForcePositive(end - now);

        return FormatTemplate(GetRatio(now, start, end), template, new Dictionary<string, string>
        {
            { "elapsed", elapsed.Humanize() },
            { "remaining", remaining.Humanize() }
        });
    }


    public static TimeSpan ForcePositive(TimeSpan ts) => ts < TimeSpan.Zero ? TimeSpan.Zero : ts;

    public static double ForceRange(double percentage, bool isUnit = false) => (percentage, isUnit) switch
    {
        ( < 0, _) => 0,
        ( > 100, false) => 100,
        ( > 1, true) => 1,
        _ => percentage
    };

    private static string FormatTemplate(double ratio, string template, Dictionary<string, string> templateValues = default)
    {
        var percentage = GetFormattedPercentage(ratio);
        var filledTemplate = template.Replace(@"{percentage}", percentage);
        if (templateValues is not null)
            foreach (var (key, value) in templateValues)
                filledTemplate = filledTemplate.Replace(@$"{{{key}}}", value);

        return filledTemplate;
    }
    public static double GetRatio(DateTime now, DateTime start, DateTime end) => ForceRange((now - start) / (end - start), isUnit: true);

    private static string GetFormattedPercentage(double ratio)
    {
        var percentage = ratio * 100;
        var percentageDesc = $"{percentage:F}%";
        return percentageDesc;
    }

    public static double GetPercentage(DateTime start, DateTime end)
    {
        var now = DateTime.Now;
        if (now > end)
            return 100;
        else if (now < start)
            return 0;
        else
            return ForceRange((now - start) * 100 / (end - start));
    }
}