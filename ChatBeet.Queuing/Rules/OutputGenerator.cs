using ChatBeet.Queuing.Rules.OutputBases;
using ChatBeet.Queuing.Rules.OutputPipes;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Queuing.Rules
{
    public class OutputGenerator : IViewable
    {
        public IOutputBase Base { get; set; }
        public IEnumerable<IOutputPipe> Pipes { get; set; }

        public string GenerateOutput(IQueuedMessageSource message)
        {
            var text = Base.GetOutput(message);
            if (Pipes != null)
                foreach (var pipe in Pipes)
                    text = pipe.Transform(text);
            return text;
        }

        public ViewableNode ToNode()
        {
            return new ViewableNode
            {
                Text = "Generate string",
                Children = GetChildren()
            };

            IEnumerable<ViewableNode> GetChildren()
            {
                yield return Base.ToNode();
                foreach (var node in Pipes.Select(p => p.ToNode()))
                    yield return node;
            }
        }
    }
}
