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
        private static Dictionary<int, Regex> keywords;
        private static bool starting;

        public KeywordRule(IOptions<IrcBotConfiguration> opts, KeywordService service)
        {
            config = opts.Value;
            this.service = service;
            if (!starting)
            {
                Task.Run(() => Initialize(service));
            }
        }

        public override bool Matches(PrivateMessage incomingMessage) => !incomingMessage.Message.StartsWith(config.CommandPrefix) && IsReady();

        public override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var tasks = keywords
                .Where(pair => pair.Value.IsMatch(incomingMessage.Message))
                .Select(pair => service.RecordKeywordEntryAsync(pair.Key, incomingMessage));

            await Task.WhenAll(tasks);
            yield break;
        }

        private bool IsReady() => keywords != default;

        private async Task Initialize(KeywordService service)
        {
            starting = true;
            var keywords = await service.GetKeywordsAsync();
            KeywordRule.keywords = keywords.ToDictionary(k => k.Id, k => new Regex(k.Regex));
        }
    }
}
