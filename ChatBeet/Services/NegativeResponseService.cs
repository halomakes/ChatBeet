using ChatBeet.Configuration;
using ChatBeet.Utilities;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;

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
    }
}
