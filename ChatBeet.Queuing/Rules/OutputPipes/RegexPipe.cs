using System.Text.RegularExpressions;

namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public class RegexPipe : IOutputPipe, IViewable
    {
        public string Pattern { get; set; }
        public string OutputTemplate { get; set; }
        public bool IgnoreCase { get; set; } = true;

        public string Transform(string input) => IgnoreCase
            ? Regex.Replace(input, Pattern, OutputTemplate, RegexOptions.IgnoreCase)
            : Regex.Replace(input, Pattern, OutputTemplate);

        public ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Then replace \"{Pattern}\" with \"{OutputTemplate}\"" + (IgnoreCase ? ", ignoring case" : string.Empty)
        };
    }
}
