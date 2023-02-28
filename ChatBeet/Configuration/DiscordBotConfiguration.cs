using System.Collections.Generic;

namespace ChatBeet.Configuration;

public class DiscordBotConfiguration
{
    public string Token { get; set; } = null!;

    public ulong LogServer { get; set; }

    public ulong LogChannel { get; set; }

    public Dictionary<string, ulong> Channels { get; set; } = null!;
    
    public Uri InvitationLink { get; set; } = null!;
}
