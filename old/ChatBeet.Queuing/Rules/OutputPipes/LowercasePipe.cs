namespace ChatBeet.Queuing.Rules.OutputPipes
{
    public class LowercasePipe : IOutputPipe, IViewable
    {
        public string Transform(string input) => input.ToLower();

        public ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Then make lowercase"
        };
    }
}
