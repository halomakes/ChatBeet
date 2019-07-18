using ChatBeet.Queuing.Rules.Conditions;

namespace ChatBeet.Queuing.Rules
{
    public class Rule
    {
        public string Label { get; set; }
        public ICondition Condition { get; set; }
        public OutputGenerator Target { get; set; }
        public OutputType Type { get; set; }
        public OutputGenerator Output { get; set; }
    }
}
