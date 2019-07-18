namespace ChatBeet.Queuing.Rules.OutputBases
{
    public interface IOutputBase
    {
        string GetOutput(IQueuedMessageSource message);
    }
}
