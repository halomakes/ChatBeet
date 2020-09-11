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
    public class KeywordRule : AsyncMessageRuleBase<PrivateMessage>
    {
        private readonly IrcBotConfiguration config;
        private readonly KeywordService service;

        public KeywordRule(IOptions<IrcBotConfiguration> opts, KeywordService service)
        {
            config = opts.Value;
            this.service = service;
        }

        public override bool Matches(PrivateMessage incomingMessage) => !incomingMessage.Message.StartsWith(config.CommandPrefix);

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
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
            return keywords.ToDictionary(k => k.Id, k => new Regex(k.Regex));
        }
    }
}
