using System.Collections.Generic;

namespace ChatBeet.Queuing.Rules
{
    public class Rule
    {
        public string Label { get; set; }
        public ICondition Condition { get; set; }
        public string TargetChannel { get; set; }
        public OutputType Type { get; set; }
        public IOutputGenerator Output { get; set; }
        public IEnumerable<IOutputPipe> Pipes { get; set; }
    }

    public enum OutputType
    {
        Message,
        Announcement,
        Activity
    }
}
