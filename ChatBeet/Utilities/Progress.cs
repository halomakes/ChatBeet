using GravyBot;
using System;
using System.Linq;

namespace ChatBeet.Utilities
{
    public static class Progress
    {
        private static readonly int barLength = 25;

        public static string GetBar(DateTime now, DateTime start, DateTime end, string periodDescription) =>
            GetBar((now - start) / (end - start), periodDescription);

        public static string GetBar(double ratio, string periodDescription)
        {
            var segments = Convert.ToInt32(ratio * barLength);
            var percentage = ratio * 100;

            var filled = string.Concat(Enumerable.Range(0, segments).Select(_ => '█'));
            var empty = string.Concat(Enumerable.Range(0, barLength - segments).Select(_ => '░'));
            var percentageDesc = $"{IrcValues.BOLD}{percentage:F}%".Colorize(Convert.ToInt32(percentage));

            var bar = $"{IrcValues.GREEN}{filled}{IrcValues.GREY}{empty}{IrcValues.RESET}  {periodDescription} {percentageDesc} complete.";
            return bar;
        }
    }
}
