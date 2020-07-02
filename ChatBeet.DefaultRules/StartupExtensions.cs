using ChatBeet.DefaultRules.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.DefaultRules
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDefaultRules(this IServiceCollection services)
        {
            services.AddSingleton<IMessageRule, HelloRule>();
            services.AddSingleton<IMessageRule, ExceptionLoggingRule>();

            return services;
        }
    }
}
