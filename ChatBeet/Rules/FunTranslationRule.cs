using ChatBeet.Services;
using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ChatBeet.Rules
{
    public abstract class FunTranslationRule : AsyncNickLookupRule
    {
        private readonly FunTranslationService translationService;
        private readonly string language;

        public FunTranslationRule(string command, string language, FunTranslationService translationService, MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, options)
        {
            this.translationService = translationService;
            CommandName = command;
            this.language = language;
        }

        protected override async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var translated = await translationService.TranslateAsync(lookupMessage.Message, language);
            if (!string.IsNullOrEmpty(translated))
                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"<{lookupMessage.From}> {translated}");
            else
                yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{incomingMessage.From}: Sorry, couldn't translate.  (Probably rate-limit exceeded.)");
        }
        public class YodaTranslationRule : FunTranslationRule
        {
            public YodaTranslationRule(FunTranslationService trans, MessageQueueService queue, IOptions<IrcBotConfiguration> opts) : base("yoda", "yoda", trans, queue, opts) { }
        }

        public class JarjarTranslationRule : FunTranslationRule
        {
            public JarjarTranslationRule(FunTranslationService trans, MessageQueueService queue, IOptions<IrcBotConfiguration> opts) : base("jarjar", "gungan", trans, queue, opts) { }
        }

        public class PirateTranslationRule : FunTranslationRule
        {
            public PirateTranslationRule(FunTranslationService trans, MessageQueueService queue, IOptions<IrcBotConfiguration> opts) : base("jarjar", "gungan", trans, queue, opts) { }
        }

        public class PigLatinTranslationRule : FunTranslationRule
        {
            public PigLatinTranslationRule(FunTranslationService trans, MessageQueueService queue, IOptions<IrcBotConfiguration> opts) : base("piglatin", "pig-latin", trans, queue, opts) { }
        }

        public class GrootTranslationRule : FunTranslationRule
        {
            public GrootTranslationRule(FunTranslationService trans, MessageQueueService queue, IOptions<IrcBotConfiguration> opts) : base("groot", "groot", trans, queue, opts) { }
        }

        public static void Register(BotRulePipeline pipeline)
        {
            pipeline.RegisterAsyncRule<YodaTranslationRule, PrivateMessage>();
            pipeline.RegisterAsyncRule<JarjarTranslationRule, PrivateMessage>();
            pipeline.RegisterAsyncRule<PirateTranslationRule, PrivateMessage>();
            pipeline.RegisterAsyncRule<PigLatinTranslationRule, PrivateMessage>();
            pipeline.RegisterAsyncRule<GrootTranslationRule, PrivateMessage>();
        }
    }
}