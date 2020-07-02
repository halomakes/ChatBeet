using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public class AndCondition : JoiningCondition, ICondition, IViewable
    {
        public override bool Matches(IQueuedMessageSource message) => Conditions == null || !Conditions.Any(c => !c.Matches(message));

        public override ViewableNode ToNode() => new ViewableNode
        {
            Text = "Satisfies all",
            Children = (Conditions ?? new List<ICondition>()).Select(c =>
            {
                if (c is IViewable)
                    return (c as IViewable).ToNode();
                return null;
            }).Where(n => n != null)
        };
    }
}
