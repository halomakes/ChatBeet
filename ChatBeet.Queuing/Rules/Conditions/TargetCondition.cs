using System;
using System.Linq.Expressions;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public class TargetCondition : PropertyCondition, ICondition, IViewable
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Target; } }

        public override ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Target matches {Match}"
        };
    }
}
