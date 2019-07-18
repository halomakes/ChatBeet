using ChatBeet.Queuing.Rules.OutputBases;
using ChatBeet.Queuing.Rules.OutputPipes;
using System.Collections.Generic;

namespace ChatBeet.Queuing.Rules
{
    public class OutputGenerator
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
    }
}
