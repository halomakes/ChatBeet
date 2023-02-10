using ChatBeet.Configuration;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class NegativeResponseService
{
    private readonly ChatBeetConfiguration _config;

    public NegativeResponseService(IOptions<ChatBeetConfiguration> opts)
    {
        _config = opts.Value;
    }

    public string GetResponseString() => _config.NegativeResponses.PickRandom();

    public Task Respond(BaseContext ctx) => ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
        .WithContent(_config.NegativeResponses.PickRandom()));

    public Task Respond(DiscordMessage message) => message.RespondAsync(_config.NegativeResponses.PickRandom());
}