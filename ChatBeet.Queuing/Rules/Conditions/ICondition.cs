namespace ChatBeet.Queuing.Rules.Conditions
{
    public interface ICondition
    {
        bool Matches(IQueuedMessageSource message);
    }
}
