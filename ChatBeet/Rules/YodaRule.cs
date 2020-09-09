using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public class YodaRule : AsyncNickLookupRule
    {
        private readonly FunTranslationService translationService;

        public YodaRule(FunTranslationService translationService, MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, options)
        {
            this.translationService = translationService;
            CommandName = "yoda";
        }

        protected override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var translated = await translationService.TranslateAsync(lookupMessage.Message, "yoda");

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {translated}");
        }
    }
}