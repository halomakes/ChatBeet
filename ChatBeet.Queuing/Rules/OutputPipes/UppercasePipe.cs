namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public class UppercasePipe : IOutputPipe, IViewable
    {
        public string Transform(string input) => input.ToUpper();

        public ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Then make uppercase"
        };
    }
}
