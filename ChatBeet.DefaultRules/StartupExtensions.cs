using ChatBeet.DefaultRules.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.DefaultRules
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDefaultRules(this IServiceCollection services)
        {
            services.AddTransient<IMessageRule, HelloRule>();
            services.AddTransient<IMessageRule, ExceptionLoggingRule>();

            return services;
        }
    }
}
