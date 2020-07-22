using ChatBeet.DefaultRules.Rules;
using ChatBeet.Irc;
using GravyIrc.Messages;

namespace ChatBeet.DefaultRules
{
    public static class StartupExtensions
    {
        public static BotRulePipeline AddDefaultRules(this BotRulePipeline pipeline)
        {
            pipeline.RegisterRule<HelloRule, PrivateMessage>();
            pipeline.RegisterRule<ExceptionLoggingRule, ExceptionMessage>();

            return pipeline;
        }
    }
}
