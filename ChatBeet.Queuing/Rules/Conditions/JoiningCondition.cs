using System.Collections.Generic;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public abstract class JoiningCondition : ICondition
    {
        public IEnumerable<ICondition> Conditions { get; set; }

        public abstract bool Matches(IQueuedMessageSource message);
    }
}
