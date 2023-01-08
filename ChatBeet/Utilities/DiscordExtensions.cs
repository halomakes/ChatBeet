using DSharpPlus.Entities;

namespace ChatBeet.Utilities
{
    public static class DiscordExtensions
    {
        public static string DiscriminatedUsername(this DiscordUser user) => $"{user.Username}#{user.Discriminator}";
    }
}
