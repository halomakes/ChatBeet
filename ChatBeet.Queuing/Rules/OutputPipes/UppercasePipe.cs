namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public class UppercasePipe : IOutputPipe
    {
        public string Transform(string input) => input.ToUpper();
    }
}
