using ChatBeet.Configuration;
using ChatBeet.Services;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatBeet.Rules
{
    public class KeywordRule : IAsyncMessageRule<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly ChatBeetConfiguration cbConfig;
        private readonly KeywordService service;

        public KeywordRule(IOptions<IrcBotConfiguration> opts, KeywordService service, IOptions<ChatBeetConfiguration> cbOpts)
        {
            config = opts.Value;
            cbConfig = cbOpts.Value;
            this.service = service;
        }

        public bool Matches(PrivateMessage incomingMessage) => incomingMessage.IsChannelMessage
            && !incomingMessage.Message.StartsWith(config.CommandPrefix)
            && cbConfig.MessageCollection.AllowedChannels.Contains(incomingMessage.To);

        public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var keywords = await GetKeywords();
            var tasks = keywords
                .Where(pair => pair.Value.IsMatch(incomingMessage.Message))
                .Select(pair => service.RecordKeywordEntryAsync(pair.Key, incomingMessage));

            await Task.WhenAll(tasks);
            yield break;
        }

        private async Task<Dictionary<int, Regex>> GetKeywords()
        {
            var keywords = await service.GetKeywordsAsync();
            return keywords.ToDictionary(k => k.Id, k => new Regex(k.Regex, RegexOptions.IgnoreCase));
        }
    }
}
