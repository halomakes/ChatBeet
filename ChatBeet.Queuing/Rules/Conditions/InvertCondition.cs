namespace ChatBeet.Queuing.Rules.Conditions
{
    public class InvertCondition : ICondition, IViewable
    {
        public ICondition Condition { get; set; }

        public bool Matches(IQueuedMessageSource message) => !Condition.Matches(message);

        public ViewableNode ToNode() => new ViewableNode
        {
            Text = "Negate result"
        };
    }
}
