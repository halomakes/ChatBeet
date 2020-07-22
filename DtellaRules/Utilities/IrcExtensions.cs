using GravyIrc.Messages;

namespace DtellaRules.Utilities
{
    public static class IrcExtensions
    {
        public static string GetResponseTarget(this PrivateMessage message) => message.IsChannelMessage ? message.To : message.From;
    }
}
