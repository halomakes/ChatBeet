using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ChatBeet.Queuing.Rules.Conditions
{
    public abstract class PropertyCondition : ICondition
    {
        public string Match { get; set; }
        public bool IgnoreCase { get; set; } = true;
        protected virtual Expression<Func<IQueuedMessageSource, string>> Accessor { get; }
        public bool Matches(IQueuedMessageSource message)
        {
            if (string.IsNullOrEmpty(Match))
                return false;
            var method = Accessor.Compile();
            var value = method(message);
            if (string.IsNullOrEmpty(value))
                return false;
            var regex = IgnoreCase ? new Regex(Match, RegexOptions.IgnoreCase) : new Regex(Match);
            return regex.IsMatch(value);
        }
    }
}
