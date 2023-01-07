using ChatBeet.Attributes;
using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot.Commands;
using GravyIrc.Messages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChatBeet.Commands.Irc
{
    public class ComplimentCommandProcessor : CommandProcessor
    {
        private readonly ComplimentService service;

        public ComplimentCommandProcessor(ComplimentService service)
        {
            this.service = service;
        }

        [Command("compliment {nick}", Description = "Pay someone a nice (or awkward) compliment.")]
        [ChannelOnly, RateLimit(10, TimeUnit.Second)]
        public async Task<IClientMessage> Analyze([Required, Nick] string nick)
        {
            var compliment = await service.GetComplimentAsync();
            return new PrivateMessage(IncomingMessage.GetResponseTarget(), $"{nick}: {compliment}");
        }
    }
}
