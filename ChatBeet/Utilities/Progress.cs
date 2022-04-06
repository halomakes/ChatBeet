using GravyBot;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Utilities
{
    public static class Progress
    {
        private static readonly int barLength = 25;

        public static string GetDescription(DateTime now, DateTime start, DateTime end, string periodDescription) =>
            GetDescription(GetRatio(now, start, end), periodDescription);

        public static string GetDescription(double ratio, string periodDescription)
        {
            var (percentage, bar) = GetPercentAndBar(ratio);

            return $"{bar}  {periodDescription} {percentage} complete.";
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

        public static double ForceRange(double percentage) => percentage switch
        {
            < 0 => 0,
            > 100 => 100,
            _ => percentage
        };

        private static string FormatTemplate(double ratio, string template, Dictionary<string, string> templateValues = default)
        {
            var (percentage, bar) = GetPercentAndBar(ratio);
            var filledTemplate = template.Replace(@"{percentage}", percentage);
            if (templateValues is not null)
                foreach (var (key, value) in templateValues)
                    filledTemplate = filledTemplate.Replace(@$"{{{key}}}", value);

            return $"{bar}  {filledTemplate}";
        }

        private static double GetRatio(DateTime now, DateTime start, DateTime end) => ForceRange((now - start) / (end - start));

        private static (string percentage, string bar) GetPercentAndBar(double ratio)
        {
            var segments = ratio switch
            {
                >= 1 => barLength,
                <= 0 => 0,
                _ => Convert.ToInt32(ratio * barLength)
            };
            var percentage = ratio * 100;

            var filled = string.Concat(Enumerable.Repeat('█', segments));
            var empty = string.Concat(Enumerable.Repeat('░', barLength - segments));
            var percentageDesc = $"{IrcValues.BOLD}{percentage:F}%".Colorize(Convert.ToInt32(percentage));
            var bar = $"{IrcValues.GREEN}{filled}{IrcValues.GREY}{empty}{IrcValues.RESET}";

            return (percentageDesc, bar);
        }
    }
}
