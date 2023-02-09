using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChatBeet.TagHelpers;

public class BackgroundBarTagHelper : TagHelper
{
    public int Value { get; set; } = 0;
    public int CollectionMax { get; set; } = 100;
    public int AnimationDelay { get; set; } = 0;

    private static readonly string className = "background-bar";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        var classList = output.Attributes.TryGetAttribute("class", out var existingClasses) ? $"{existingClasses.Value} {className}" : className;
        output.Attributes.SetAttribute("class", classList);
        var styleAttrib = $"width: {GetPercentage()}%; animation-delay: {AnimationDelay}ms";
        output.Attributes.SetAttribute("style", styleAttrib);
    }

    private float GetPercentage() => CollectionMax > 0 ? (float)Value * 100 / CollectionMax : 0;
}