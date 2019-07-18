using System.Text.RegularExpressions;

namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public class RegexPipe : IOutputPipe
    {
        public string Pattern { get; set; }
        public string OutputTemplate { get; set; }
        public bool IgnoreCase { get; set; } = true;

        public string Transform(string input) => IgnoreCase
            ? Regex.Replace(input, Pattern, OutputTemplate, RegexOptions.IgnoreCase)
            : Regex.Replace(input, Pattern, OutputTemplate);
    }
}
