namespace ChatBeet.Queuing.Rules.OutputPipes
{

    public class TrimPipe : IOutputPipe, IViewable
    {
        public string Transform(string input) => input.Trim();

        public ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Then trim whitespace"
        };
    }
}
