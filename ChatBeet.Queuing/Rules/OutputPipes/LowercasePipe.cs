namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public class LowercasePipe : IOutputPipe
    {
        public string Transform(string input) => input.ToLower();
    }
}
