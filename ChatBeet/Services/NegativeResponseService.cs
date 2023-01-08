using ChatBeet.Configuration;
using ChatBeet.Utilities;
using DSharpPlus.SlashCommands;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class NegativeResponseService
    {
        private readonly ChatBeetConfiguration config;

        public NegativeResponseService(IOptions<ChatBeetConfiguration> opts)
        {
            config = opts.Value;
        }

        public IClientMessage GetResponse(string target, string nick) => new PrivateMessage(target, $"{nick}: {config.NegativeResponses.PickRandom()}");

        public IClientMessage GetResponse(PrivateMessage pm) => GetResponse(pm.GetResponseTarget(), pm.From);

        public Task Respond(InteractionContext ctx) => ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder()
            .WithContent(config.NegativeResponses.PickRandom()));
    }
}
