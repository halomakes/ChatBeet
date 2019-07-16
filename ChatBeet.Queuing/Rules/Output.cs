using System.Text.RegularExpressions;

namespace ChatBeet.Queuing.Rules
{
    public interface IOutputGenerator
    {
        string GetOutput(IQueuedMessageSource message);
    }

    public class TemplatedOutputGenerator : IOutputGenerator
    {
        public string Template { get; set; }

        public string GetOutput(IQueuedMessageSource message)
        {
            var output = Template;
            if (string.IsNullOrEmpty(output))
                return string.Empty;
            return ReplaceTemplateProperties(output, message);
        }

        private string ReplaceTemplateProperties(string input, IQueuedMessageSource message)
        {
            foreach (var prop in message.GetType().GetProperties())
                input = input.Replace($"{{{{{prop.Name}}}}}", prop.GetValue(message)?.ToString() ?? string.Empty);
            return input;
        }
    }

    public interface IOutputPipe
    {
        string Transform(string input);
    }

    public class RegexPipe : IOutputPipe
    {
        public string Pattern { get; set; }
        public string OutputTemplate { get; set; }

        public string Transform(string input) => Regex.Replace(input, Pattern, OutputTemplate);
    }

    public class UppercasePipe : IOutputPipe
    {
        public string Transform(string input) => input.ToUpper();
    }

    public class LowercasePipe : IOutputPipe
    {
        public string Transform(string input) => input.ToLower();
    }

    public class TrimPipe : IOutputPipe
    {
        public string Transform(string input) => input.Trim();
    }
}
