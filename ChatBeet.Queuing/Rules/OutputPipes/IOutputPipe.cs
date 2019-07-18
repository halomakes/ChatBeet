namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public interface IOutputPipe : IViewable
    {
        string Transform(string input);
    }
}
