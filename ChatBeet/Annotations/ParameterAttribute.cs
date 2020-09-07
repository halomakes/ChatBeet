using System;

namespace ChatBeet.Annotations
{
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(string InlineName, string DisplayName = default)
        {
            this.InlineName = InlineName;
            this.DisplayName = DisplayName;
        }

        public string DisplayName { get; set; }

        public string InlineName { get; set; }
    }
}
