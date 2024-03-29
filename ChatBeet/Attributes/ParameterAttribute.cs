﻿namespace ChatBeet.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ParameterAttribute : Attribute
{
    public ParameterAttribute(string inlineName, string? displayName = default)
    {
        InlineName = inlineName;
        DisplayName = displayName;
    }

    public string? DisplayName { get; set; }

    public string InlineName { get; set; }
}