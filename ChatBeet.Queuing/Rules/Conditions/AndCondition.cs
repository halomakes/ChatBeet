using System.Linq;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public class AndCondition : JoiningCondition, ICondition
    {
        public override bool Matches(IQueuedMessageSource message) => Conditions == null || !Conditions.Any(c => !c.Matches(message));
    }
}
