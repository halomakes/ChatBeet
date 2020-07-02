using ChatBeet.Queuing.Rules.Conditions;
using System.Collections.Generic;

namespace ChatBeet.Queuing.Rules
{
    public class Rule : IViewable
    {
        public string Label { get; set; }
        public ICondition Condition { get; set; }
        public OutputGenerator Target { get; set; }
        public OutputType Type { get; set; }
        public OutputGenerator Output { get; set; }

        public ViewableNode ToNode()
        {
            return new ViewableNode
            {
                Text = Label,
                Children = GetChildren()
            };

            IEnumerable<ViewableNode> GetChildren()
            {
                yield return new ViewableNode
                {
                    Text = "If",
                    Children = new List<ViewableNode> { Condition?.ToNode() }
                };
                yield return new ViewableNode
                {
                    Text = "Send",
                    Children = new List<ViewableNode> { Output?.ToNode() }
                };
                yield return new ViewableNode
                {
                    Text = "To",
                    Children = new List<ViewableNode> { Target?.ToNode() }
                };
                yield return new ViewableNode
                {
                    Text = $"As {Type}"
                };
            }
        }
    }
}
