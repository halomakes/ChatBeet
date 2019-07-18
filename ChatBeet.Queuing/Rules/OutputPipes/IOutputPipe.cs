namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public interface IOutputPipe
    {
        string Transform(string input);
    }
}
