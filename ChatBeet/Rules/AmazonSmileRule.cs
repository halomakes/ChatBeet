using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;

public partial class AmazonSmileRule : IMessageRule<PrivateMessage>
{
    [GeneratedRegex(@"((?:https?:\/\/)?(?:www.amazon\.com)\/\S+)")]
    private static partial Regex rgx();

    public bool Matches(PrivateMessage incomingMessage) =>
        incomingMessage.IsChannelMessage && rgx().IsMatch(incomingMessage.Message);

    public IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage)
    {
        var match = rgx().Match(incomingMessage.Message);
        if (match.Success)
        {
            var url = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(url))
            {
                var modified = ModifyUri(url);
                if (!string.IsNullOrEmpty(modified))
                    yield return new PrivateMessage(incomingMessage.GetResponseTarget(), modified);
            }
        }
    }

    /// <summary>
    /// Changes domain to smile.amazon.com and strips any tracking info from query
    /// </summary>
    private static string ModifyUri(string original)
    {
        try
        {
            var originalBuilder = new UriBuilder(original);
            return new UriBuilder
            {
                Host = "smile.amazon.com",
                Scheme = "https",
                Path = originalBuilder.Path
            }.ToString();
        }
        catch
        {
            return default;
        }
    }
}