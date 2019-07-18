namespace ChatBeet.Queuing.Rules.OutputBases
{
    public interface IOutputBase : IViewable
    {
        string GetOutput(IQueuedMessageSource message);
    }
}
