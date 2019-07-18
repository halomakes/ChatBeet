namespace ChatBeet.Queuing.Rules.Conditions
{
    public interface ICondition : IViewable
    {
        bool Matches(IQueuedMessageSource message);
    }
}
