using System;
using System.Linq.Expressions;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public class SourceCondition : PropertyCondition, ICondition, IViewable
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Source; } }

        public override ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Source matches {Match}"
        };
    }
}
