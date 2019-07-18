using System;
using System.Linq.Expressions;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public class TitleCondition : PropertyCondition, ICondition
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Title; } }
    }
}
