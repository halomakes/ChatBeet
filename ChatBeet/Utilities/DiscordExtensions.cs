using DSharpPlus.Entities;

namespace ChatBeet.Utilities
{
    public static class DiscordExtensions
    {
        public static string DiscriminatedUsername(this DiscordUser user) => $"{user.Username}#{user.Discriminator}";

        public static (bool Success, string Username, string Discriminator) ParseUsername(this string username)
        {
            var hashLocation = username.IndexOf('#');
            if (hashLocation < 0)
                return (false, default, default);
            var partialUsername = username[..hashLocation];
            var discriminator = username[(hashLocation + 1)..];
            return (true, partialUsername, discriminator);
        }
    }
}
