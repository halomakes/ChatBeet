using System.Collections.Generic;

namespace ChatBeet.Queuing.Rules
{
    public class ViewableNode
    {
        public string Text { get; set; }
        public IEnumerable<ViewableNode> Children { get; set; }
    }
}
