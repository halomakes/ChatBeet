using ChatBeet.Configuration;
using ChatBeet.Utilities;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class NegativeResponseService
{
    private readonly ChatBeetConfiguration config;

    public NegativeResponseService(IOptions<ChatBeetConfiguration> opts)
    {
        config = opts.Value;
    }

    public string GetResponseString() => config.NegativeResponses.PickRandom();

    public IClientMessage GetResponse(string target, string nick) => new PrivateMessage(target, $"{nick}: {config.NegativeResponses.PickRandom()}");

    public IClientMessage GetResponse(PrivateMessage pm) => GetResponse(pm.GetResponseTarget(), pm.From);

    public Task Respond(BaseContext ctx) => ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
        .WithContent(config.NegativeResponses.PickRandom()));

    public Task Respond(DiscordMessage message) => message.RespondAsync(config.NegativeResponses.PickRandom());
}