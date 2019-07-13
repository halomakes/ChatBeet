using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBeet.Queuing.Rules
{
    public class Rule
    {
        public string Label { get; set; }
        public ICondition Condition { get; set; }
        public string TargetChannel { get; set; }
        public IOutputGenerator Output { get; set; }
    }
}
