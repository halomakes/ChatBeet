namespace ChatBeet.Queuing.Rules.OutputPipes
{

    public class TrimPipe : IOutputPipe
    {
        public string Transform(string input) => input.Trim();
    }
}
