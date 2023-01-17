using ChatBeet.Utilities;
using DSharpPlus;
using DSharpPlus.EventArgs;
using GravyBot;
using GravyIrc.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules;
public partial class EightBallRule : IAsyncMessageRule<MessageCreateEventArgs>
{
    private readonly DiscordClient _discord;

    public EightBallRule(DiscordClient discord)
    {
        _discord = discord;
    }

    [GeneratedRegex(@"^\<@\d+\>\W+\w+.*\?$", RegexOptions.IgnoreCase)]
    private static partial Regex discordRgx();

    public bool Matches(MessageCreateEventArgs incomingMessage) => discordRgx().IsMatch(incomingMessage.Message.Content)
        && incomingMessage.MentionedUsers.FirstOrDefault() == _discord.CurrentUser;

    public async IAsyncEnumerable<IClientMessage> RespondAsync(MessageCreateEventArgs incomingMessage)
    {
        await incomingMessage.Message.RespondAsync(YesNoGenerator.GetResponse());
        yield break;
    }
}
