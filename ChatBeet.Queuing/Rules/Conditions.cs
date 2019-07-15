using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ChatBeet.Queuing.Rules
{
    public interface ICondition
    {
        bool Matches(IQueuedMessageSource message);
    }

    public abstract class PropertyCondition : ICondition
    {
        public string Match { get; set; }
        protected virtual Expression<Func<IQueuedMessageSource, string>> Accessor { get; }
        public bool Matches(IQueuedMessageSource message)
        {
            var method = Accessor.Compile();
            return new Regex(Match).IsMatch(method(message));
        }
    }

    public class SourceCondition : PropertyCondition, ICondition
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Source; } }
    }

    public class BodyCondition : PropertyCondition, ICondition
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Body; } }
    }

    public class TargetCondition : PropertyCondition, ICondition
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Target; } }
    }

    public class TitleCondition : PropertyCondition, ICondition
    {
        protected override Expression<Func<IQueuedMessageSource, string>> Accessor { get { return c => c.Title; } }
    }

    public abstract class JoiningCondition : ICondition
    {
        public IEnumerable<ICondition> Conditions { get; set; }

        public abstract bool Matches(IQueuedMessageSource message);
    }

    public class AndCondition : JoiningCondition, ICondition
    {
        public override bool Matches(IQueuedMessageSource message) => Conditions == null || !Conditions.Any(c => !c.Matches(message));
    }

    public class OrCondition : JoiningCondition, ICondition
    {
        public override bool Matches(IQueuedMessageSource message) => Conditions == null || Conditions.Any(c => c.Matches(message));
    }
}
