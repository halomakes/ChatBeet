using System;
using System.Linq.Expressions;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public class TitleCondition : PropertyCondition, ICondition, IViewable
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Title; } }

        public override ViewableNode ToNode() => new ViewableNode
        {
            Text = $"Title matches {Match}"
        };
    }
}
