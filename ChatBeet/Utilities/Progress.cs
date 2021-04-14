using GravyBot;
using System;
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

        public static string FormatTemplate(DateTime now, DateTime start, DateTime end, string template) =>
            FormatTemplate(GetRatio(now, start, end), template);

        public static string FormatTemplate(double ratio, string template)
        {
            var (percentage, bar) = GetPercentAndBar(ratio);
            var filledTemplate = template.Replace(@"{percentage}", percentage);

            return $"{bar}  {filledTemplate}";
        }

        private static double GetRatio(DateTime now, DateTime start, DateTime end) => (now - start) / (end - start);

        private static (string percentage, string bar) GetPercentAndBar(double ratio)
        {
            var segments = Convert.ToInt32(ratio * barLength);
            var percentage = ratio * 100;

            var filled = string.Concat(Enumerable.Range(0, segments).Select(_ => '█'));
            var empty = string.Concat(Enumerable.Range(0, barLength - segments).Select(_ => '░'));
            var percentageDesc = $"{IrcValues.BOLD}{percentage:F}%".Colorize(Convert.ToInt32(percentage));
            var bar = $"{IrcValues.GREEN}{filled}{IrcValues.GREY}{empty}{IrcValues.RESET}";

            return (percentageDesc, bar);
        }
    }
}
