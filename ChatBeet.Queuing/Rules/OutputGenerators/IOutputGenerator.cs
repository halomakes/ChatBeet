namespace ChatBeet.Queuing.Rules.OutputGenerators
{
    public interface IOutputGenerator
    {
        string GetOutput(IQueuedMessageSource message);
    }
}
