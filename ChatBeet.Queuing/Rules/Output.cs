namespace ChatBeet.Queuing.Rules
{
    public interface IOutputGenerator
    {
        string GetOutput(IQueuedMessageSource message);
    }

    public class TemplatedOutputGenerator : IOutputGenerator
    {
        public string Template { get; set; }

        public string GetOutput(IQueuedMessageSource message) => string.Format(Template, message);
    }
}
