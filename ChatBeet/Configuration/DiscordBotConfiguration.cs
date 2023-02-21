using System.Collections.Generic;

namespace ChatBeet.Configuration;

public class DiscordBotConfiguration
{
    public string Token { get; set; }

    public ulong LogServer { get; set; }

    public ulong LogChannel { get; set; }

    public Dictionary<string, ulong> Channels { get; set; }
    
    public Uri InvitationLink { get; set; }
}
